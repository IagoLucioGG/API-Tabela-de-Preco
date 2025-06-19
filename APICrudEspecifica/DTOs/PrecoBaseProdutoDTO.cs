namespace APICrudEspecifica.DTOs
{
    public class PrecoBaseProdutoDTO
    {
        public decimal ValorBase { get; set; }
        public DateOnly DataInicioVigencia { get; set; }
        public DateOnly DataFimVigencia { get; set; }
        public bool Ativo { get; set; }
        public int IdProduto { get; set; }
        public int IdFamilia { get; set; }
        public int IdMovimentoComercial { get; set; }
    }
}
