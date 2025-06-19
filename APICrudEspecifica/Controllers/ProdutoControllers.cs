using Microsoft.AspNetCore.Mvc;
using APICrudEspecifica.Models;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using APICrudEspecifica.Exceptions;

namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        
        private readonly AppDbContext _context;
        

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post (ProdutoDTO dto)
        {
            try
            {
                var chaveExternaExistente = await _context.Produtos.FirstOrDefaultAsync(p => p.IdChaveExterna.ToLower() == dto.IdChaveExterna.ToLower());

                if(chaveExternaExistente != null)
                {
                    return Conflict(new { Mensagem = ExceptionProduto.ProdutoDuplicadoChave });
                }
                var novoProduto = new Produto
                {
                    NomeProduto = dto.NomeProduto,
                    Ativo = dto.Ativo? dto.Ativo : true,
                    CodigoProduto = dto.CodigoProduto,
                    IdFamilia = dto.IdFamilia,
                    IdChaveExterna = dto.IdChaveExterna,
                };

                _context.Produtos.Add(novoProduto);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetById), new { idProduto = novoProduto.IdProduto }, novoProduto);

            }catch (Exception ex)
            {
                return StatusCode(500, new {Mensagem = "Algo deu errado no servidor.", Erro =  ex.Message});
            }
        }

        [HttpGet("{idProduto}")]
        public async Task<ActionResult<Produto>> GetById(int idProduto)
        {
            var produto = await _context.Produtos.FindAsync(idProduto);

            if(produto == null)
            {
                return NotFound(new {Mensagem = ExceptionProduto.ProdutoNaoEncontrado});
            }

            return Ok(produto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _context.Produtos.ToListAsync();
            
            if(produtos == null)
            {
                return NotFound(new { Mensagem = ExceptionProduto.ProdutoNaoEncontrado});
            }

            return Ok(produtos);
        }

        [HttpGet("filtrar-produtos")]
        public async Task<ActionResult<IEnumerable<Produto>>> Buscar(
                [FromQuery] string? nomeProduto,
                [FromQuery] int? idProduto,
                [FromQuery] bool? ativo,
                [FromQuery] string? idChaveExterna
            ) 
        {
            var query =  _context.Produtos.AsQueryable();

            if (idProduto.HasValue)
            {
                query = query.Where(p => p.IdProduto == idProduto);   
            }
            if (!string.IsNullOrWhiteSpace(nomeProduto))
            {
                query = query.Where(p => p.NomeProduto.ToLower().Contains(nomeProduto.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(idChaveExterna)){
                query = query.Where(p => p.IdChaveExterna ==  idChaveExterna);
            }

            if (ativo.HasValue)
            {
                query = query.Where(p => p.Ativo == ativo.Value);
            }

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new { Mensagem = ExceptionProduto.ProdutoNaoEncontrado });
            }

            return Ok(resultados);
        }

        



        [HttpPut("{idProduto}")]
        public async Task<ActionResult> Put(int idProduto, ProdutoDTO dto)
        {
            var nomeExistente = await _context.Produtos.AnyAsync(p => p.NomeProduto.Equals(dto.NomeProduto) && p.IdProduto != idProduto);

            if(nomeExistente)
            {
                return Conflict(new { Mensagem = ExceptionProduto.ProdutoDuplicado });
            }

            var produto = await _context.Produtos.FindAsync(idProduto);

            if(produto == null)
            {
                return NotFound(new { Mensagem = ExceptionProduto.ProdutoNaoEncontrado });
            }

            produto.NomeProduto = dto.NomeProduto;
            produto.Ativo = dto.Ativo ? dto.Ativo : true;
            produto.CodigoProduto = dto.CodigoProduto;
            produto.IdFamilia = dto.IdFamilia;
            produto.IdChaveExterna = dto.IdChaveExterna;

            await _context.SaveChangesAsync();

            return Ok(produto);
        }


        [HttpDelete("{idProduto}")]
        public async Task<ActionResult> Delete(int idProduto)
        {
            var produto = await _context.Produtos.FindAsync(idProduto);

            if(produto == null)
            {
                return NotFound(new { Mensagem = ExceptionProduto.ProdutoNaoEncontrado });
            }

             _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(new {Mensagem = ExceptionProduto.ProdutoDeletado});
        }
    }
}
