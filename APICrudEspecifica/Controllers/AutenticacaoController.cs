using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APICrudEspecifica.Data;
using APICrudEspecifica.DTOs;
using APICrudEspecifica.Models;
using APICrudEspecifica.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace APICrudEspecifica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AutenticacaoController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.NomeUsuario) || string.IsNullOrEmpty(loginDto.Senha))
                return BadRequest(new { Mensagem = "Usuário e senha são obrigatórios." });

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NomeUsuario == loginDto.NomeUsuario);

            if (usuario == null)
                return Unauthorized(new { Mensagem = "Usuário ou senha inválidos." });

            bool senhaValida = SenhaHasher.VerificarSenha(loginDto.Senha, usuario.SenhaHash);
            if (!senhaValida)
                return Unauthorized(new { Mensagem = "Usuário ou senha inválidos." });

            var token = GerarToken(usuario);

            return Ok(new { Token = token });
        }

        [Authorize(Policy = "Cadastrar")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioCadastroDTO dto)
        {
            if (string.IsNullOrEmpty(dto.NomeUsuario) || string.IsNullOrEmpty(dto.Senha))
            {
                return BadRequest(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Nome de usuário e senha são obrigatórios.",
                    Dados = null
                });
            }

            var existe = await _context.Usuarios.AnyAsync(u => u.NomeUsuario == dto.NomeUsuario);
            if (existe)
            {
                return Conflict(new ResponseModel<object>
                {
                    Status = false,
                    Mensagem = "Nome de usuário já está em uso.",
                    Dados = null
                });
            }

            if (dto.Permissoes == "ADMIN")
            {
                dto.Permissoes = "CONSULTAR,DELETAR,CADASTRAR,EDITAR";
            }

            var usuario = new Usuario
            {
                NomeUsuario = dto.NomeUsuario,
                SenhaHash = SenhaHasher.GerarHash(dto.Senha),
                Ativo = true,
                Permissoes = dto.Permissoes ?? ""
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<object>
            {
                Status = true,
                Mensagem = "Usuário criado com sucesso.",
                Dados = null // ou enviar dados resumidos do usuário, se quiser
            });
        }



        private string GerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("Permissoes", usuario.Permissoes ?? "")
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
