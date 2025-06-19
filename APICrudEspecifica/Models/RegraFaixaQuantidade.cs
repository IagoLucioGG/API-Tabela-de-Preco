using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICrudEspecifica.Models
{
    public class RegraFaixaQuantidade
    {
        [Key]
        public int IdRegraFaixaQuantidade { get; set; }

        [Required]
        public int IdRegraPrecificacao { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoMedida { get; set; } = string.Empty;

        [Required]
        public int QtdMin { get; set; }

        public int? QtdMax { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [MaxLength(500)]
        public string? Observacao { get; set; }

       
        
    }
}
