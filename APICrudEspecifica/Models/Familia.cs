using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class Familia
    {
        [Key]
        public int IdFamilia { get; set; }
        public string NomeFamilia { get; set; } = string.Empty;
        

    }
}
