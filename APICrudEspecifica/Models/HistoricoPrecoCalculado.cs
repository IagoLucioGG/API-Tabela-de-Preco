using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICrudEspecifica.Models
{
    public class HistoricoPrecoCalculado
    {
        [Key]
        public int IdHistoricoPreco { get; set; }

        [Required]
        public int IdProduto { get; set; }

        [Required]
        public int IdFamilia { get; set; }

        public int? QtdParametro { get; set; }

        [MaxLength(255)]
        public string? CondicionalParametro { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAdesaoFinal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAssinaturaFinal { get; set; }

        [Required]
        public DateTime DataCalculo { get; set; }

        public string? RegrasAplicadasJSON { get; set; }

        [MaxLength(255)]
        public string? UsuarioConsulta { get; set; }

    }
}
