using APICrudEspecifica.Models;
using Microsoft.EntityFrameworkCore;

namespace APICrudEspecifica.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<MovimentoComercial> Movimentos { get; set; }
        public DbSet<PrecoBaseProduto> PrecosBaseProdutos { get; set; }
        public DbSet<RegraFaixaQuantidade> RegrasFaixaQuantidade { get; set; }
        public DbSet<RegraPrecificacao> RegrasPrecificacao { get; set; }
        public DbSet<RegraCondicional> RegrasCondicional { get; set; }
        public DbSet<HistoricoPrecoCalculado> HistoricosPrecos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}
