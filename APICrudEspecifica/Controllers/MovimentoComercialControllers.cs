using APICrudEspecifica.Data;
using APICrudEspecifica.Models;
using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.DTOs;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

namespace APICrudEspecifica.Controllers
{

    [ApiController]
    [Route("api/movimento")]
    public class MovimentoComercialController : ControllerBase
    {

        private readonly AppDbContext _context;
        public MovimentoComercialController(AppDbContext context)
        {
            _context = context;
        }


        

        [HttpPost]
        public async Task<ActionResult> Post(MovimentoComercialDTO dto)
        {
            var movimentoExistente = await _context.Movimentos.AnyAsync(m => m.TipoMovimentoComercial == dto.TipoMovimentoComercial);
            if (movimentoExistente)
            {
                return Conflict(new { Mensagem = ExceptionMovimentoComercial.TipoComercialJaExiste });
            }

            var novoMovimento = new MovimentoComercial()
            {
                TipoMovimentoComercial = dto.TipoMovimentoComercial,
                DescricaoMovComercial = dto.DescricaoMovComercial
            };

            _context.Add(novoMovimento);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { idMovimentoComercial = novoMovimento.IdMovimentoComercial }, novoMovimento);

        }

        [HttpGet("{idMovimento}")]
        public async Task<ActionResult<MovimentoComercial>> GetById(int idMovimento)
        {
            var movimento = await _context.Movimentos.FindAsync(idMovimento);

            if(movimento == null)
            {
                return NotFound(new { Mensagem = "Não foi encontrado nenhum movimento na base."});
            }

            return Ok(movimento);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentoComercial>>> Get()
        {
            var movimentos = await _context.Movimentos.ToListAsync();
            return Ok(movimentos);
        }
    }
}
