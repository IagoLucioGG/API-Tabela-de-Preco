using APICrudEspecifica.Data;
using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/familia")]
    public class FamiliaControllers : ControllerBase
    {
        private readonly AppDbContext _context;

        public FamiliaControllers(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Familia>>> Get()
        {
            var familia = await _context.Familias.ToListAsync();

            if(familia == null)
            {
                return NotFound(new { Mensagem = ExceptionFamilia.FamiliaNaoEncontrada });
            }
            return Ok(familia);
        }

        [HttpGet("{idFamilia}")]
        public async Task<ActionResult<Familia>> GetById(int idFamilia)
        {
            var familia = await _context.Familias.FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            if (familia == null)
            {
                return NotFound(new { Mensagem = ExceptionFamilia.FamiliaNaoEncontrada });
            }
            
            return Ok(familia);
        }
    }
}
