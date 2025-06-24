using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Data;
using APICrudEspecifica.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using APICrudEspecifica.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/ConsultarPreco")]
    public class ConsultaPrecoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultaPrecoController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Policy = "Consultar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<object>>> Post([FromBody] ConsultarPrecoDTO dados)
        {
            // Etapa 1: Obter Produto e Família (sem alterações)
            var produtoFamiliaInfo = await (
                from p in _context.Produtos
                join f in _context.Familias on p.IdFamilia equals f.IdFamilia
                where f.NomeFamilia == dados.NomeFamilia
                      && p.IdChaveExterna.ToLower() == dados.IdChaveExternaProduto.ToLower()
                select new
                {
                    p.IdProduto,
                    f.IdFamilia,
                    p.NomeProduto,
                    p.CodigoProduto,
                    f.NomeFamilia
                }).FirstOrDefaultAsync();

            if (produtoFamiliaInfo == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Produto ou família não encontrados.",
                    Dados = null
                });
            }

            // Etapa 3: Buscar dados para cálculo (precisa antecipar para decidir condicionais usadas)
            var registrosBrutos = await (
                from rp in _context.RegrasPrecificacao
                join m in _context.Movimentos on rp.IdTipoMovimentoComercial equals m.IdMovimentoComercial
                join pp in _context.PrecosBaseProdutos on new { rp.IdProduto, m.IdMovimentoComercial } equals new { pp.IdProduto, pp.IdMovimentoComercial } into jpp
                from pp in jpp.DefaultIfEmpty()
                join rc in _context.RegrasCondicional on rp.IdRegraPrecificacao equals rc.IdRegraPrecificacao into jrc
                from rc in jrc.DefaultIfEmpty()
                join rf in _context.RegrasFaixaQuantidade on rp.IdRegraPrecificacao equals rf.IdRegraPrecificacao
                where rp.IdProduto == produtoFamiliaInfo.IdProduto
                      && rp.IdFamilia == produtoFamiliaInfo.IdFamilia
                      && rf.QtdMin <= dados.QtFaixaQuantidade
                      && rf.QtdMax >= dados.QtFaixaQuantidade
                select new
                {
                    produtoFamiliaInfo.NomeProduto,
                    produtoFamiliaInfo.CodigoProduto,
                    Descricao = m.DescricaoMovComercial,
                    ValorBase = pp != null ? pp.ValorBase : (decimal?)null,
                    ValorCondicional = rc != null ? rc.ValorAjuste : (decimal?)null,
                    CondicaoTipo = rc != null ? rc.CondicaoTipo : null,
                    CondicaoValorEsperado = rc != null ? rc.CondicaoValorEsperado : null
                }
            ).ToListAsync();

            if (!registrosBrutos.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Nenhum dado encontrado para os parâmetros informados.",
                    Dados = null
                });
            }

            // Etapa 4: Identificar as condicionais que serão usadas para cálculo (com base nos dados e regras)
            var condicionaisUsadas = new List<CondicionalDTO>();
            var partes = new List<string>();

            foreach (var mov in dados.DescricoesMovComercial)
            {
                var registrosDoMovimento = registrosBrutos
                    .Where(r => r.Descricao == mov)
                    .ToList();

                decimal? valorFinal = null;

                foreach (var r in registrosDoMovimento)
                {
                    var condicional = dados.Condicionais.FirstOrDefault(c =>
                        r.CondicaoTipo == c.CondicaoTipo &&
                        r.CondicaoValorEsperado == c.CondicaoValorEsperado);

                    if (condicional != null && r.ValorCondicional.HasValue)
                    {
                        valorFinal = r.ValorCondicional;
                        if (!condicionaisUsadas.Any(cu => cu.CondicaoTipo == condicional.CondicaoTipo && cu.CondicaoValorEsperado == condicional.CondicaoValorEsperado))
                            condicionaisUsadas.Add(condicional);
                        break;
                    }

                    if (r.ValorBase.HasValue)
                    {
                        valorFinal = r.ValorBase;
                    }
                }

                var valorFormatado = valorFinal?.ToString("0.##") ?? "";
                partes.Add($"{mov}:{valorFormatado}");
            }

            var resultadoFormatado = string.Join("---", partes);

            // Etapa 2 (ajustada): Antes de calcular, tenta buscar histórico com condicionais usadas
            if (!dados.ForcarCalculo)
            {
                // Ordena e serializa as condicionais usadas para comparar no banco
                var condicoesOrdenadas = condicionaisUsadas
                    .OrderBy(c => c.CondicaoTipo)
                    .ThenBy(c => c.CondicaoValorEsperado)
                    .ToList();

                var condicoesJson = JsonSerializer.Serialize(condicoesOrdenadas);

                var historico = await _context.HistoricosPrecos
                    .Where(h => h.IdProduto == produtoFamiliaInfo.IdProduto &&
                                h.IdFamilia == produtoFamiliaInfo.IdFamilia &&
                                h.QuantidadeFaixa == dados.QtFaixaQuantidade &&
                                h.CondicionaisUtilizadasJSON == condicoesJson)
                    .OrderByDescending(h => h.DataCalculo)
                    .FirstOrDefaultAsync();

                if (historico != null && (DateTime.Now - historico.DataCalculo).TotalDays <= 7)
                {
                    // Extrai os movimentos do resultado formatado (ex: "Venda:10---Troca:20")
                    var movimentosHistorico = historico.ResultadoFormatado
                        .Split("---", StringSplitOptions.RemoveEmptyEntries)
                        .Select(m => m.Split(":")[0].Trim())
                        .ToList();

                    // Normaliza ambas listas para comparação
                    var recebidos = dados.DescricoesMovComercial.Select(m => m.Trim()).ToList();

                    bool listasIguais = !movimentosHistorico.Except(recebidos).Any()
                                        && !recebidos.Except(movimentosHistorico).Any();

                    if (listasIguais)
                    {
                        var condicionalLista = JsonSerializer.Deserialize<List<CondicionalDTO>>(historico.CondicionaisUtilizadasJSON);

                        return Ok(new ResponseModel<object>
                        {
                            Status = true,
                            Mensagem = "Consulta retornada do histórico.",
                            Dados = new
                            {
                                Produto = new { historico.NomeProduto, historico.CodigoProduto },
                                CondicionalUtilizada = condicionalLista,
                                historico.ResultadoFormatado
                            }
                        });
                    }
                }

            }

            // Etapa 5: Salvar histórico com as condicionais usadas
            var historicoSalvar = new HistoricoPrecoCalculado
            {
                IdProduto = produtoFamiliaInfo.IdProduto,
                IdFamilia = produtoFamiliaInfo.IdFamilia,
                NomeProduto = produtoFamiliaInfo.NomeProduto,
                CodigoProduto = produtoFamiliaInfo.CodigoProduto,
                NomeFamilia = produtoFamiliaInfo.NomeFamilia,
                QuantidadeFaixa = dados.QtFaixaQuantidade,
                ResultadoFormatado = resultadoFormatado,
                CondicionaisUtilizadasJSON = condicionaisUsadas.Any()
                    ? JsonSerializer.Serialize(
                        condicionaisUsadas.OrderBy(c => c.CondicaoTipo).ThenBy(c => c.CondicaoValorEsperado).ToList())
                    : null,
                RegrasAplicadasJSON = null,
                DataCalculo = DateTime.Now,
                UsuarioConsulta = User?.Identity?.Name ?? "Sistema"
            };

            _context.HistoricosPrecos.Add(historicoSalvar);
            await _context.SaveChangesAsync();

            // Etapa 6: Retorno final
            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = "Cálculo realizado com sucesso.",
                Dados = new
                {
                    Produto = new
                    {
                        produtoFamiliaInfo.NomeProduto,
                        produtoFamiliaInfo.CodigoProduto
                    },
                    CondicionalUtilizada = condicionaisUsadas,
                    resultadoFormatado
                }
            });
        }



    }
}
