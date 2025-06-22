using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        [MaxLength(200)]
        public string? Permissoes { get; set; } // Exemplo: "CONSULTAR,HISTORICO,ADMIN"
    }
}
