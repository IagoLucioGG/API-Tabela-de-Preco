using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.DTOs
{
    public class ConsultarPrecoDTO
    {
        [Required]
        public List<string> DescricoesMovComercial { get; set; } = new();

        [Required]
        public string NomeFamilia { get; set; } = string.Empty;

        [Required]
        public string IdChaveExternaProduto { get; set; } = string.Empty;
        [Required]
        public int QtFaixaQuantidade {  get; set; }

        public List<CondicionalDTO> Condicionais { get; set; } = new();
        public bool ForcarCalculo { get; set; } = false;

    }
}
