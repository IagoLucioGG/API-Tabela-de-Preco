using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/regra-precificacao")]
    public class RegraPrecificacaoControllers : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegraPrecificacaoControllers(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<RegraPrecificacaoDTO>> Post(RegraPrecificacaoDTO dto)
        {
            try
            {
                var regra = new RegraPrecificacao
                {
                    IdProduto = dto.IdProduto,
                    IdFamilia = dto.IdFamilia,
                    TipoRegra = dto.TipoRegra,
                    NomeRegra = dto.NomeRegra,
                    Descricao = dto.Descricao,
                    DataInicioVigencia = dto.DataInicioVigencia,
                    DataFimVigencia = dto.DataFimVigencia,
                    Prioridade = dto.Prioridade,
                    Ativo = dto.Ativo.HasValue ? dto.Ativo.Value : true,
                    IdTipoMovimentoComercial = dto.IdTipoMovimentoComercial
                };

                _context.RegrasPrecificacao.Add(regra);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = regra.IdRegraPrecificacao }, regra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Erro = ex.Message });
            }
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RegraPrecificacao>> GetById(int id)
        {
            var regra = await _context.RegrasPrecificacao.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraPrecificacao.RegraNaoEncontrada });
            }

            return Ok(regra);
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegraPrecificacao>>> Get()
        {
            var regras = await _context.RegrasPrecificacao.ToListAsync();

            if (!regras.Any())
            {
                return NotFound(new { Mensagem = ExceptionRegraPrecificacao.RegraNaoEncontrada });
            }

            return Ok(regras);
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<RegraPrecificacao>>> Filtrar(
            [FromQuery] int? idProduto,
            [FromQuery] int? idFamilia,
            [FromQuery] string? tipoRegra,
            [FromQuery] string? nomeRegra,
            [FromQuery] bool? ativo
        )
        {
            var query = _context.RegrasPrecificacao.AsQueryable();

            if (idProduto.HasValue)
                query = query.Where(r => r.IdProduto == idProduto.Value);

            if (idFamilia.HasValue)
                query = query.Where(r => r.IdFamilia == idFamilia.Value);

            if (!string.IsNullOrWhiteSpace(tipoRegra))
                query = query.Where(r => r.TipoRegra.ToLower().Contains(tipoRegra.ToLower()));

            if (!string.IsNullOrWhiteSpace(nomeRegra))
                query = query.Where(r => r.NomeRegra.ToLower().Contains(nomeRegra.ToLower()));

            if (ativo.HasValue)
                query = query.Where(r => r.Ativo == ativo.Value);

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new { Mensagem = ExceptionRegraPrecificacao.RegraNaoEncontrada });
            }

            return Ok(resultados);
        }

        [Authorize(Policy = "Editar")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, RegraPrecificacaoDTO dto)
        {
            var regra = await _context.RegrasPrecificacao.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraPrecificacao.RegraNaoEncontrada });
            }

            regra.IdProduto = dto.IdProduto;
            regra.IdFamilia = dto.IdFamilia;
            regra.TipoRegra = dto.TipoRegra;
            regra.NomeRegra = dto.NomeRegra;
            regra.Descricao = dto.Descricao;
            regra.DataInicioVigencia = dto.DataInicioVigencia;
            regra.DataFimVigencia = dto.DataFimVigencia;
            regra.Prioridade = dto.Prioridade;
            regra.Ativo = dto.Ativo.HasValue ? dto.Ativo.Value : true;
            regra.IdTipoMovimentoComercial = dto.IdTipoMovimentoComercial;

            await _context.SaveChangesAsync();

            return Ok(regra);
        }

        [Authorize(Policy = "Deletar")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var regra = await _context.RegrasPrecificacao.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraPrecificacao.RegraNaoEncontrada });
            }

            _context.RegrasPrecificacao.Remove(regra);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = ExceptionRegraPrecificacao.RegraRemovida });
        }
    }
}
