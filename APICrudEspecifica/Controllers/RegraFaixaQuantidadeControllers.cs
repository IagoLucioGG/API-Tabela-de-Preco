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
    [Route("api/regra-faixa")]
    public class RegraFaixaQuantidadeControllers : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegraFaixaQuantidadeControllers(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<RegraFaixaQuantidade>>> Post(RegraFaixaQuantidadeDTO dto)
        {
            try
            {
                var regra = new RegraFaixaQuantidade
                {
                    IdRegraPrecificacao = dto.IdRegraPrecificacao,
                    TipoMedida = dto.TipoMedida,
                    QtdMin = dto.QtdMin,
                    QtdMax = dto.QtdMax,
                    Valor = dto.Valor,
                    Observacao = dto.Observacao
                };

                _context.RegrasFaixaQuantidade.Add(regra);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = regra.IdRegraFaixaQuantidade }, new ResponseModel<RegraFaixaQuantidade>
                {
                    Status = true,
                    Mensagem = "Regra de faixa cadastrada com sucesso.",
                    Dados = regra
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Erro interno no servidor.",
                    Dados = ex.Message
                });
            }
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<RegraFaixaQuantidade>>> GetById(int id)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<RegraFaixaQuantidade>
            {
                Status = true,
                Mensagem = "Regra de faixa encontrada com sucesso.",
                Dados = regra
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<RegraFaixaQuantidade>>>> Get()
        {
            var regras = await _context.RegrasFaixaQuantidade.ToListAsync();

            if (!regras.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<RegraFaixaQuantidade>>
            {
                Status = true,
                Mensagem = "Regras de faixa encontradas com sucesso.",
                Dados = regras
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("filtrar")]
        public async Task<ActionResult<ResponseModel<IEnumerable<RegraFaixaQuantidade>>>> Filtrar(
            [FromQuery] int? idRegraPrecificacao,
            [FromQuery] string? tipoMedida,
            [FromQuery] int? qtdMin,
            [FromQuery] int? qtdMax)
        {
            var query = _context.RegrasFaixaQuantidade.AsQueryable();

            if (idRegraPrecificacao.HasValue)
                query = query.Where(r => r.IdRegraPrecificacao == idRegraPrecificacao.Value);

            if (!string.IsNullOrWhiteSpace(tipoMedida))
                query = query.Where(r => r.TipoMedida.ToLower().Contains(tipoMedida.ToLower()));

            if (qtdMin.HasValue)
                query = query.Where(r => r.QtdMin >= qtdMin.Value);

            if (qtdMax.HasValue)
                query = query.Where(r => r.QtdMax <= qtdMax.Value || r.QtdMax == null);

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<RegraFaixaQuantidade>>
            {
                Status = true,
                Mensagem = "Regras de faixa filtradas com sucesso.",
                Dados = resultados
            });
        }

        [Authorize(Policy = "Editar")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<RegraFaixaQuantidade>>> Put(int id, RegraFaixaQuantidadeDTO dto)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada,
                    Dados = null
                });
            }

            regra.IdRegraPrecificacao = dto.IdRegraPrecificacao;
            regra.TipoMedida = dto.TipoMedida;
            regra.QtdMin = dto.QtdMin;
            regra.QtdMax = dto.QtdMax;
            regra.Valor = dto.Valor;
            regra.Observacao = dto.Observacao;

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<RegraFaixaQuantidade>
            {
                Status = true,
                Mensagem = "Regra de faixa atualizada com sucesso.",
                Dados = regra
            });
        }

        [Authorize(Policy = "Deletar")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<object>>> Delete(int id)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada,
                    Dados = null
                });
            }

            _context.RegrasFaixaQuantidade.Remove(regra);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = ExceptionRegraFaixa.RegraRemovida,
                Dados = null
            });
        }
    }
}
