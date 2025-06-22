using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICrudEspecifica.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarMinhaNovaTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Familias",
                columns: table => new
                {
                    IdFamilia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeFamilia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Familias", x => x.IdFamilia);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosPrecos",
                columns: table => new
                {
                    IdHistoricoPreco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdFamilia = table.Column<int>(type: "int", nullable: false),
                    QtdParametro = table.Column<int>(type: "int", nullable: true),
                    CondicionalParametro = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ValorAdesaoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorAssinaturaFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataCalculo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegrasAplicadasJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioConsulta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosPrecos", x => x.IdHistoricoPreco);
                });

            migrationBuilder.CreateTable(
                name: "Movimentos",
                columns: table => new
                {
                    IdMovimentoComercial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMovimentoComercial = table.Column<int>(type: "int", nullable: false),
                    DescricaoMovComercial = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentos", x => x.IdMovimentoComercial);
                });

            migrationBuilder.CreateTable(
                name: "PrecosBaseProdutos",
                columns: table => new
                {
                    IdPrecoBaseProduto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataInicioVigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFimVigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdFamilia = table.Column<int>(type: "int", nullable: false),
                    IdMovimentoComercial = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecosBaseProdutos", x => x.IdPrecoBaseProduto);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    IdProduto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProduto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoProduto = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdChaveExterna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdFamilia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.IdProduto);
                });

            migrationBuilder.CreateTable(
                name: "RegrasCondicional",
                columns: table => new
                {
                    IdRegraCondicional = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRegraPrecificacao = table.Column<int>(type: "int", nullable: false),
                    CondicaoTipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CondicaoValorEsperado = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValorAjuste = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoAjuste = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegrasCondicional", x => x.IdRegraCondicional);
                });

            migrationBuilder.CreateTable(
                name: "RegrasFaixaQuantidade",
                columns: table => new
                {
                    IdRegraFaixaQuantidade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRegraPrecificacao = table.Column<int>(type: "int", nullable: false),
                    TipoMedida = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QtdMin = table.Column<int>(type: "int", nullable: false),
                    QtdMax = table.Column<int>(type: "int", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegrasFaixaQuantidade", x => x.IdRegraFaixaQuantidade);
                });

            migrationBuilder.CreateTable(
                name: "RegrasPrecificacao",
                columns: table => new
                {
                    IdRegraPrecificacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdFamilia = table.Column<int>(type: "int", nullable: false),
                    TipoRegra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomeRegra = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicioVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFimVigencia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Prioridade = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdTipoMovimentoComercial = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegrasPrecificacao", x => x.IdRegraPrecificacao);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Familias");

            migrationBuilder.DropTable(
                name: "HistoricosPrecos");

            migrationBuilder.DropTable(
                name: "Movimentos");

            migrationBuilder.DropTable(
                name: "PrecosBaseProdutos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "RegrasCondicional");

            migrationBuilder.DropTable(
                name: "RegrasFaixaQuantidade");

            migrationBuilder.DropTable(
                name: "RegrasPrecificacao");
        }
    }
}
