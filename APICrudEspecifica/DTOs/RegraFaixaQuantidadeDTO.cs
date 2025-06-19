namespace APICrudEspecifica.DTOs
{
    public class RegraFaixaQuantidadeDTO
    {
        public int IdRegraPrecificacao { get; set; }
        public string TipoMedida { get; set; } = string.Empty;
        public int QtdMin { get; set; }
        public int? QtdMax { get; set; }
        public decimal Valor { get; set; }
        public string? Observacao { get; set; }
    }
}
