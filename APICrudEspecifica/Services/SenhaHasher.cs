using System.Security.Cryptography;
using System.Text;

namespace APICrudEspecifica.Services
{
    public static class SenhaHasher
    {
        public static string GerarHash(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerificarSenha(string senhaTexto, string hashSalvo)
        {
            var hashTexto = GerarHash(senhaTexto);
            return hashTexto == hashSalvo;
        }
    }
}
