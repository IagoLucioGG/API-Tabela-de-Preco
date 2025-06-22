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
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<Produto>>> Post(ProdutoDTO dto)
        {
            try
            {
                var chaveExternaExistente = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.IdChaveExterna.ToLower() == dto.IdChaveExterna.ToLower());

                if (chaveExternaExistente != null)
                {
                    return Conflict(new ResponseModel<object>
                    {
                        Status = false,
                        Mensagem = ExceptionProduto.ProdutoDuplicadoChave,
                        Dados = null
                    });
                }

                var novoProduto = new Produto
                {
                    NomeProduto = dto.NomeProduto,
                    Ativo = dto.Ativo,
                    CodigoProduto = dto.CodigoProduto,
                    IdFamilia = dto.IdFamilia,
                    IdChaveExterna = dto.IdChaveExterna
                };

                _context.Produtos.Add(novoProduto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { idProduto = novoProduto.IdProduto }, new ResponseModel<Produto>
                {
                    Status = true,
                    Mensagem = "Produto criado com sucesso.",
                    Dados = novoProduto
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
        [HttpGet("{idProduto}")]
        public async Task<ActionResult<ResponseModel<Produto>>> GetById(int idProduto)
        {
            var produto = await _context.Produtos.FindAsync(idProduto);

            if (produto == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<Produto>
            {
                Status = true,
                Mensagem = "Produto encontrado com sucesso.",
                Dados = produto
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<Produto>>>> Get()
        {
            var produtos = await _context.Produtos.ToListAsync();

            if (!produtos.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<Produto>>
            {
                Status = true,
                Mensagem = "Produtos encontrados com sucesso.",
                Dados = produtos
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("filtrar-produtos")]
        public async Task<ActionResult<ResponseModel<IEnumerable<Produto>>>> Buscar(
            [FromQuery] string? nomeProduto,
            [FromQuery] int? idProduto,
            [FromQuery] bool? ativo,
            [FromQuery] string? idChaveExterna)
        {
            var query = _context.Produtos.AsQueryable();

            if (idProduto.HasValue)
                query = query.Where(p => p.IdProduto == idProduto);

            if (!string.IsNullOrWhiteSpace(nomeProduto))
                query = query.Where(p => p.NomeProduto.ToLower().Contains(nomeProduto.ToLower()));

            if (!string.IsNullOrWhiteSpace(idChaveExterna))
                query = query.Where(p => p.IdChaveExterna == idChaveExterna);

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            var resultados = await query.ToListAsync();

            if (!resultados.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<Produto>>
            {
                Status = true,
                Mensagem = "Produtos filtrados com sucesso.",
                Dados = resultados
            });
        }

        [Authorize(Policy = "Editar")]
        [HttpPut("{idProduto}")]
        public async Task<ActionResult<ResponseModel<Produto>>> Put(int idProduto, ProdutoDTO dto)
        {
            var nomeExistente = await _context.Produtos
                .AnyAsync(p => p.NomeProduto == dto.NomeProduto && p.IdProduto != idProduto);

            if (nomeExistente)
            {
                return Conflict(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoDuplicado,
                    Dados = null
                });
            }

            var produto = await _context.Produtos.FindAsync(idProduto);

            if (produto == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoNaoEncontrado,
                    Dados = null
                });
            }

            produto.NomeProduto = dto.NomeProduto;
            produto.Ativo = dto.Ativo;
            produto.CodigoProduto = dto.CodigoProduto;
            produto.IdFamilia = dto.IdFamilia;
            produto.IdChaveExterna = dto.IdChaveExterna;

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<Produto>
            {
                Status = true,
                Mensagem = "Produto atualizado com sucesso.",
                Dados = produto
            });
        }

        [Authorize(Policy = "Deletar")]
        [HttpDelete("{idProduto}")]
        public async Task<ActionResult<ResponseModel<object>>> Delete(int idProduto)
        {
            var produto = await _context.Produtos.FindAsync(idProduto);

            if (produto == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionProduto.ProdutoNaoEncontrado,
                    Dados = null
                });
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = ExceptionProduto.ProdutoDeletado,
                Dados = null
            });
        }
    }
}
