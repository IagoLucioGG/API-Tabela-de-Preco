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
    [Route("api/regra-condicional")]
    public class RegraCondicionalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegraCondicionalController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<RegraCondicional>>> Post(RegraCondicionalDTO dto)
        {
            try
            {
                var regra = new RegraCondicional
                {
                    IdRegraPrecificacao = dto.IdRegraPrecificacao,
                    CondicaoTipo = dto.CondicaoTipo,
                    CondicaoValorEsperado = dto.CondicaoValorEsperado,
                    ValorAjuste = dto.ValorAjuste,
                    TipoAjuste = dto.TipoAjuste
                };

                _context.RegrasCondicional.Add(regra);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = regra.IdRegraCondicional }, new ResponseModel<RegraCondicional>
                {
                    Status = true,
                    Mensagem = "Regra condicional cadastrada com sucesso.",
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
        public async Task<ActionResult<ResponseModel<RegraCondicional>>> GetById(int id)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<RegraCondicional>
            {
                Status = true,
                Mensagem = "Regra condicional encontrada com sucesso.",
                Dados = regra
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<RegraCondicional>>>> Get()
        {
            var regras = await _context.RegrasCondicional.ToListAsync();

            if (!regras.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<RegraCondicional>>
            {
                Status = true,
                Mensagem = "Regras condicionais encontradas com sucesso.",
                Dados = regras
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("filtrar")]
        public async Task<ActionResult<ResponseModel<IEnumerable<RegraCondicional>>>> Filtrar(
            [FromQuery] int? idRegraPrecificacao,
            [FromQuery] string? condicaoTipo,
            [FromQuery] string? tipoAjuste)
        {
            var query = _context.RegrasCondicional.AsQueryable();

            if (idRegraPrecificacao.HasValue)
                query = query.Where(r => r.IdRegraPrecificacao == idRegraPrecificacao.Value);

            if (!string.IsNullOrWhiteSpace(condicaoTipo))
                query = query.Where(r => r.CondicaoTipo.ToLower().Contains(condicaoTipo.ToLower()));

            if (!string.IsNullOrWhiteSpace(tipoAjuste))
                query = query.Where(r => r.TipoAjuste.ToLower().Contains(tipoAjuste.ToLower()));

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<RegraCondicional>>
            {
                Status = true,
                Mensagem = "Regras condicionais filtradas com sucesso.",
                Dados = resultados
            });
        }

        [Authorize(Policy = "Editar")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<RegraCondicional>>> Put(int id, RegraCondicionalDTO dto)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada,
                    Dados = null
                });
            }

            regra.IdRegraPrecificacao = dto.IdRegraPrecificacao;
            regra.CondicaoTipo = dto.CondicaoTipo;
            regra.CondicaoValorEsperado = dto.CondicaoValorEsperado;
            regra.ValorAjuste = dto.ValorAjuste;
            regra.TipoAjuste = dto.TipoAjuste;

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<RegraCondicional>
            {
                Status = true,
                Mensagem = "Regra condicional atualizada com sucesso.",
                Dados = regra
            });
        }

        [Authorize(Policy = "Deletar")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<object>>> Delete(int id)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada,
                    Dados = null
                });
            }

            _context.RegrasCondicional.Remove(regra);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = ExceptionRegraCondicional.RegraRemovida,
                Dados = null
            });
        }
    }
}
