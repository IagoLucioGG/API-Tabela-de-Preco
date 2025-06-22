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

        [MaxLength(255)]
        public string NomeProduto { get; set; } = string.Empty;

        [MaxLength(100)]
        public int CodigoProduto { get; set; }

        [MaxLength(255)]
        public string NomeFamilia { get; set; } = string.Empty;

        [Required]
        public int QuantidadeFaixa { get; set; }  // Era QtdParametro

        [Required]
        public string ResultadoFormatado { get; set; } = string.Empty;
        // Exemplo: "Adesão:3388---Assinatura:29.9---Implantação:---Conversão:"

        public string? CondicionaisUtilizadasJSON { get; set; }
        // Exemplo: [{"condicaoTipo":"Cliente","condicaoValorEsperado":"true"}]

        public string? RegrasAplicadasJSON { get; set; }

        [Required]
        public DateTime DataCalculo { get; set; }

        [MaxLength(255)]
        public string? UsuarioConsulta { get; set; }
    }
}
