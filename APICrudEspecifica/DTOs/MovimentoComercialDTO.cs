using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.DTOs
{
    public class MovimentoComercialDTO
    {
        public int TipoMovimentoComercial { get; set; }
        public string DescricaoMovComercial { get; set; } = string.Empty;
    }
}
