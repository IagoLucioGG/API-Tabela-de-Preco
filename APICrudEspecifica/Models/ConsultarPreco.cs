using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class ConsultarPreco
    {
        [Required]
        public List<string> DescricoesMovComercial { get; set; } = new();
        [Required]
        public string NomeFamilia { get; set; } = string.Empty;
        [Required]
        public string IdChaveExternaProduto { get; set; } = string.Empty;
        public string CondicaoTipo { get; set; } = string.Empty;
        public string CondicaoValorEsperado { get; set; } = string.Empty;
    }
}

