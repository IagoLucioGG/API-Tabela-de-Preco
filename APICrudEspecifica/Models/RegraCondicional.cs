using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICrudEspecifica.Models
{
    public class RegraCondicional
    {
        [Key]
        public int IdRegraCondicional { get; set; }

        [Required]
        public int IdRegraPrecificacao { get; set; }

        [Required]
        [MaxLength(50)]
        public string CondicaoTipo { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string CondicaoValorEsperado { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAjuste { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoAjuste { get; set; } = string.Empty;
    }
}
