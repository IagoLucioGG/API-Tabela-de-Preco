using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class MovimentoComercial
    {
        [Key]
        public int IdMovimentoComercial { get; set; }

        //Tipos de movimentos possíveis: 1 - xxx / 2 - zzz / 2 - zzz / 2 - zzz
        [Required]
        public int TipoMovimentoComercial { get; set; }

        public string DescricaoMovComercial { get; set; } = string.Empty;
    }
}
