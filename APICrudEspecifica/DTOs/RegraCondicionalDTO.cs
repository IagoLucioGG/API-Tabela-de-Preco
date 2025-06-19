namespace APICrudEspecifica.DTOs
{
    public class RegraCondicionalDTO
    {
        public int IdRegraPrecificacao { get; set; }
        public string CondicaoTipo { get; set; } = string.Empty;
        public string CondicaoValorEsperado { get; set; } = string.Empty;
        public decimal ValorAjuste { get; set; }
        public string TipoAjuste { get; set; } = string.Empty;
    }
}
