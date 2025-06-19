using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.Models
{
    public class PrecoBaseProduto
    {
        [Key]
        public int IdPrecoBaseProduto { get; set; }
        public decimal ValorBase {  get; set; }
        public DateOnly DataInicioVigencia { get; set; }
        public DateOnly DataFimVigencia { get; set; }
        [Required]
        public bool Ativo {  get; set; }
        public int IdProduto { get; set; }
        public int IdFamilia { get; set; }
        public int IdMovimentoComercial { get; set; }

    }
}
