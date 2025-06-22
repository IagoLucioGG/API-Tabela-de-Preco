namespace APICrudEspecifica.DTOs
{
    public class UsuarioCadastroDTO
    {
        public string NomeUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Permissoes { get; set; } // opcional, ex: "CONSULTAR,HISTORICO"
    }
}
