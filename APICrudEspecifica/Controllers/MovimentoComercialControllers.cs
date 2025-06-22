using APICrudEspecifica.Data;
using APICrudEspecifica.Models;
using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.DTOs;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<MovimentoComercial>>> Post(MovimentoComercialDTO dto)
        {
            var movimentoExistente = await _context.Movimentos
                .AnyAsync(m => m.TipoMovimentoComercial == dto.TipoMovimentoComercial);

            if (movimentoExistente)
            {
                return Conflict(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionMovimentoComercial.TipoComercialJaExiste,
                    Dados = null
                });
            }

            var novoMovimento = new MovimentoComercial
            {
                TipoMovimentoComercial = dto.TipoMovimentoComercial,
                DescricaoMovComercial = dto.DescricaoMovComercial
            };

            _context.Movimentos.Add(novoMovimento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { idMovimentoComercial = novoMovimento.IdMovimentoComercial }, new ResponseModel<MovimentoComercial>
            {
                Status = true,
                Mensagem = "Movimento comercial criado com sucesso.",
                Dados = novoMovimento
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("{idMovimentoComercial}")]
        public async Task<ActionResult<ResponseModel<MovimentoComercial>>> GetById(int idMovimentoComercial)
        {
            var movimento = await _context.Movimentos.FindAsync(idMovimentoComercial);

            if (movimento == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Não foi encontrado nenhum movimento na base.",
                    Dados = null
                });
            }

            return Ok(new ResponseModel<MovimentoComercial>
            {
                Status = true,
                Mensagem = "Movimento encontrado com sucesso.",
                Dados = movimento
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<MovimentoComercial>>>> Get()
        {
            var movimentos = await _context.Movimentos.ToListAsync();

            return Ok(new ResponseModel<IEnumerable<MovimentoComercial>>
            {
                Status = true,
                Mensagem = "Movimentos encontrados com sucesso.",
                Dados = movimentos
            });
        }
    }
}
