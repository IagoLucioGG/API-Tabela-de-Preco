using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class RegraPrecificacao
    {
        [Key]
        public int IdRegraPrecificacao { get; set; }

        public int IdProduto { get; set; }

        public int IdFamilia { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoRegra { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string NomeRegra { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Required]
        public DateTime DataInicioVigencia { get; set; }

        public DateTime? DataFimVigencia { get; set; }

        [Required]
        public int Prioridade { get; set; }

        [Required]
        public bool Ativo { get; set; }

        public int? IdTipoMovimentoComercial { get; set; }
    }
}
