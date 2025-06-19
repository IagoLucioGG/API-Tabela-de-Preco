using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

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

        [HttpPost]
        public async Task<ActionResult<RegraFaixaQuantidadeDTO>> Post(RegraFaixaQuantidadeDTO dto)
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

                return CreatedAtAction(nameof(GetById), new { id = regra.IdRegraFaixaQuantidade }, regra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Erro = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegraFaixaQuantidade>> GetById(int id)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada });
            }

            return Ok(regra);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegraFaixaQuantidade>>> Get()
        {
            var regras = await _context.RegrasFaixaQuantidade.ToListAsync();

            if (!regras.Any())
            {
                return NotFound(new { Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada });
            }

            return Ok(regras);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<RegraFaixaQuantidade>>> Filtrar(
            [FromQuery] int? idRegraPrecificacao,
            [FromQuery] string? tipoMedida,
            [FromQuery] int? qtdMin,
            [FromQuery] int? qtdMax
        )
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
                return NotFound(new { Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada });
            }

            return Ok(resultados);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, RegraFaixaQuantidadeDTO dto)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada });
            }

            regra.IdRegraPrecificacao = dto.IdRegraPrecificacao;
            regra.TipoMedida = dto.TipoMedida;
            regra.QtdMin = dto.QtdMin;
            regra.QtdMax = dto.QtdMax;
            regra.Valor = dto.Valor;
            regra.Observacao = dto.Observacao;

            await _context.SaveChangesAsync();

            return Ok(regra);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var regra = await _context.RegrasFaixaQuantidade.FindAsync(id);

            if (regra == null)
            {
                return NotFound(new { Mensagem = ExceptionRegraFaixa.RegraNaoEncontrada });
            }

            _context.RegrasFaixaQuantidade.Remove(regra);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = ExceptionRegraFaixa.RegraRemovida });
        }
    }
}
