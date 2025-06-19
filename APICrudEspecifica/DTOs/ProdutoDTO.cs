using System.ComponentModel.DataAnnotations;

namespace APICrudEspecifica.DTOs
{
    public class ProdutoDTO
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int CodigoProduto { get; set; }
        public bool Ativo { get; set; }

        public int IdFamilia { get; set; }
        public string IdChaveExterna { get; set; } = string.Empty;

    }
}
