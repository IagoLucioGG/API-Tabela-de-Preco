using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/preco-base")]
    public class PrecoBaseProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrecoBaseProdutoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<PrecoBaseProdutoDTO>> Post(PrecoBaseProdutoDTO dto)
        {
            try
            {
                var preco = new PrecoBaseProduto
                {
                    ValorBase = dto.ValorBase,
                    DataInicioVigencia = dto.DataInicioVigencia,
                    DataFimVigencia = dto.DataFimVigencia,
                    Ativo = dto.Ativo,
                    IdProduto = dto.IdProduto,
                    IdFamilia = dto.IdFamilia,
                    IdMovimentoComercial = dto.IdMovimentoComercial
                };

                _context.PrecosBaseProdutos.Add(preco);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = preco.IdPrecoBaseProduto }, preco);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno no servidor.", Erro = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrecoBaseProduto>> GetById(int id)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new { Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado });
            }

            return Ok(preco);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrecoBaseProduto>>> Get()
        {
            var precos = await _context.PrecosBaseProdutos.ToListAsync();

            if (!precos.Any())
            {
                return NotFound(new { Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado });
            }

            return Ok(precos);
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<IEnumerable<PrecoBaseProduto>>> Filtrar(
            [FromQuery] int? idProduto,
            [FromQuery] int? idFamilia,
            [FromQuery] bool? ativo,
            [FromQuery] DateOnly? dataInicio,
            [FromQuery] DateOnly? dataFim)
        {
            var query = _context.PrecosBaseProdutos.AsQueryable();

            if (idProduto.HasValue)
                query = query.Where(p => p.IdProduto == idProduto.Value);

            if (idFamilia.HasValue)
                query = query.Where(p => p.IdFamilia == idFamilia.Value);

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            if (dataInicio.HasValue)
                query = query.Where(p => p.DataInicioVigencia == dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(p => p.DataFimVigencia == dataFim.Value);

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new { Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado });
            }

            return Ok(resultados);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, PrecoBaseProdutoDTO dto)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new { Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado });
            }

            preco.ValorBase = dto.ValorBase;
            preco.DataInicioVigencia = dto.DataInicioVigencia;
            preco.DataFimVigencia = dto.DataFimVigencia;
            preco.Ativo = dto.Ativo;
            preco.IdProduto = dto.IdProduto;
            preco.IdFamilia = dto.IdFamilia;
            preco.IdMovimentoComercial = dto.IdMovimentoComercial;

            await _context.SaveChangesAsync();

            return Ok(preco);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new { Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado });
            }

            _context.PrecosBaseProdutos.Remove(preco);
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = ExceptionPrecoBase.PrecoBaseDeletado });
        }
    }
}
