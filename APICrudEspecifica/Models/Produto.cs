using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class Produto
    {
        [Key]
        public int IdProduto { get; set; }
        [Required]
        public string NomeProduto { get; set; } = string.Empty;
        public  int CodigoProduto { get; set; }
        public bool Ativo {  get; set; }
        public string IdChaveExterna { get; set; } = string.Empty;
        public int IdFamilia { get; set; }

    }
}
