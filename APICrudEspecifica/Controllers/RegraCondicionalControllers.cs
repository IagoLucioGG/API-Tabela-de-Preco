using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

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

        [HttpPost]
        public async Task<ActionResult<RegraCondicionalDTO>> Post(RegraCondicionalDTO dto)
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

                return CreatedAtAction(nameof(GetById), new { id = regra.IdRegraCondicional }, regra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Erro = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegraCondicional>> GetById(int id)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada });
            }

            return Ok(regra);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegraCondicional>>> Get()
        {
            var regras = await _context.RegrasCondicional.ToListAsync();

            if (!regras.Any())
            {
                return NotFound(new { Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada });
            }

            return Ok(regras);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<RegraCondicional>>> Filtrar(
            [FromQuery] int? idRegraPrecificacao,
            [FromQuery] string? condicaoTipo,
            [FromQuery] string? tipoAjuste
        )
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
                return NotFound(new { Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada });
            }

            return Ok(resultados);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, RegraCondicionalDTO dto)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada });
            }

            regra.IdRegraPrecificacao = dto.IdRegraPrecificacao;
            regra.CondicaoTipo = dto.CondicaoTipo;
            regra.CondicaoValorEsperado = dto.CondicaoValorEsperado;
            regra.ValorAjuste = dto.ValorAjuste;
            regra.TipoAjuste = dto.TipoAjuste;

            await _context.SaveChangesAsync();

            return Ok(regra);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var regra = await _context.RegrasCondicional.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraCondicional.RegraNaoEncontrada });
            }

            _context.RegrasCondicional.Remove(regra);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = ExceptionRegraCondicional.RegraRemovida });
        }
    }
}
