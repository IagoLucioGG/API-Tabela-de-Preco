namespace APICrudEspecifica.DTOs
{
    public class RegraPrecificacaoDTO
    {
        public int IdProduto { get; set; }
        public int IdFamilia { get; set; }
        public string TipoRegra { get; set; } = string.Empty;
        public string NomeRegra { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public int Prioridade { get; set; }
        public bool? Ativo { get; set; }
        public int? IdTipoMovimentoComercial { get; set; }
    }
}
