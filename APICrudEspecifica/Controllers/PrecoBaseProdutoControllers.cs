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
    [Route("api/preco-base")]
    public class PrecoBaseProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrecoBaseProdutoController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<PrecoBaseProduto>>> Post(PrecoBaseProdutoDTO dto)
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

                return CreatedAtAction(nameof(GetById), new { id = preco.IdPrecoBaseProduto }, new ResponseModel<PrecoBaseProduto>
                {
                    Status = true,
                    Mensagem = "Preço base cadastrado com sucesso.",
                    Dados = preco
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
        public async Task<ActionResult<ResponseModel<PrecoBaseProduto>>> GetById(int id)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<PrecoBaseProduto>
            {
                Status = true,
                Mensagem = "Preço base encontrado com sucesso.",
                Dados = preco
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<PrecoBaseProduto>>>> Get()
        {
            var precos = await _context.PrecosBaseProdutos.ToListAsync();

            if (!precos.Any())
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<PrecoBaseProduto>>
            {
                Status = true,
                Mensagem = "Preços base encontrados com sucesso.",
                Dados = precos
            });
        }

        [Authorize(Policy = "Consultar")]
        [HttpGet("filtrar")]
        public async Task<ActionResult<ResponseModel<IEnumerable<PrecoBaseProduto>>>> Filtrar(
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
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado,
                    Dados = null
                });
            }

            return Ok(new ResponseModel<IEnumerable<PrecoBaseProduto>>
            {
                Status = true,
                Mensagem = "Preços filtrados com sucesso.",
                Dados = resultados
            });
        }

        [Authorize(Policy = "Editar")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<PrecoBaseProduto>>> Put(int id, PrecoBaseProdutoDTO dto)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado,
                    Dados = null
                });
            }

            preco.ValorBase = dto.ValorBase;
            preco.DataInicioVigencia = dto.DataInicioVigencia;
            preco.DataFimVigencia = dto.DataFimVigencia;
            preco.Ativo = dto.Ativo;
            preco.IdProduto = dto.IdProduto;
            preco.IdFamilia = dto.IdFamilia;
            preco.IdMovimentoComercial = dto.IdMovimentoComercial;

            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<PrecoBaseProduto>
            {
                Status = true,
                Mensagem = "Preço base atualizado com sucesso.",
                Dados = preco
            });
        }

        [Authorize(Policy = "Deletar")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<object>>> Delete(int id)
        {
            var preco = await _context.PrecosBaseProdutos.FindAsync(id);

            if (preco == null)
            {
                return NotFound(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = ExceptionPrecoBase.PrecoBaseNaoEncontrado,
                    Dados = null
                });
            }

            _context.PrecosBaseProdutos.Remove(preco);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = ExceptionPrecoBase.PrecoBaseDeletado,
                Dados = null
            });
        }
    }
}
