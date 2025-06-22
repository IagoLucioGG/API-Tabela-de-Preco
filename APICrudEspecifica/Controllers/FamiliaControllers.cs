using APICrudEspecifica.Data;
using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;
using APICrudEspecifica.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/familia")]
    public class FamiliaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FamiliaController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<Familia>>> Post(FamiliaDTO dto)
        {
            var familiaExistente = await _context.Familias
                .FirstOrDefaultAsync(f => f.NomeFamilia.ToLower() == dto.NomeFamilia.ToLower());

            if (familiaExistente != null)
            {
                return Conflict(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionFamilia.FamiliaExistente,
                    Dados = null
                });
            }

            var novaFamilia = new Familia
            {
                NomeFamilia = dto.NomeFamilia
            };

            _context.Familias.Add(novaFamilia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { idFamilia = novaFamilia.IdFamilia }, new ResponseModel<Familia>
            {
                Status = true,
                Mensagem = "Família criada com sucesso.",
                Dados = novaFamilia
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<Familia>>>> Get()
        {
            var familias = await _context.Familias.ToListAsync();

            if (familias == null || !familias.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionFamilia.FamiliaNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<Familia>>
            {
                Status = true,
                Mensagem = "Famílias encontradas com sucesso.",
                Dados = familias
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("{idFamilia}")]
        public async Task<ActionResult<ResponseModel<Familia>>> GetById(int idFamilia)
        {
            var familia = await _context.Familias.FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            if (familia == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionFamilia.FamiliaNaoEncontrada,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<Familia>
            {
                Status = true,
                Mensagem = "Família encontrada com sucesso.",
                Dados = familia
            });
        }
    }
}
