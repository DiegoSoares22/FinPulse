using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinPulse.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicialComSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "ContasBancarias",
                columns: table => new
                {
                    ContaBancariaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ativa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasBancarias", x => x.ContaBancariaId);
                });

            migrationBuilder.CreateTable(
                name: "MetasFinanceiras",
                columns: table => new
                {
                    MetaFinanceiraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValorAlvo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorAtual = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataLimite = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasFinanceiras", x => x.MetaFinanceiraId);
                });

            migrationBuilder.CreateTable(
                name: "Orcamentos",
                columns: table => new
                {
                    OrcamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorLimite = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    ValorGasto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamentos", x => x.OrcamentoId);
                    table.ForeignKey(
                        name: "FK_Orcamentos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    TransacaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ContaBancariaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.TransacaoId);
                    table.ForeignKey(
                        name: "FK_Transacoes_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacoes_ContasBancarias_ContaBancariaId",
                        column: x => x.ContaBancariaId,
                        principalTable: "ContasBancarias",
                        principalColumn: "ContaBancariaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "CategoriaId", "Cor", "Icone", "Nome", "Tipo" },
                values: new object[,]
                {
                    { 1, "#EF4444", "utensils", "Alimentação", 1 },
                    { 2, "#F97316", "car", "Transporte", 1 },
                    { 3, "#8B5CF6", "home", "Moradia", 1 },
                    { 4, "#EC4899", "heart-pulse", "Saúde", 1 },
                    { 5, "#EAB308", "gamepad", "Lazer", 1 },
                    { 6, "#3B82F6", "graduation-cap", "Educação", 1 },
                    { 7, "#10B981", "money-bill-wave", "Salário", 0 },
                    { 8, "#06B6D4", "chart-line", "Investimentos", 0 },
                    { 9, "#6366F1", "laptop-code", "Freelance / Extra", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orcamentos_CategoriaId",
                table: "Orcamentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaBancariaId",
                table: "Transacoes",
                column: "ContaBancariaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetasFinanceiras");

            migrationBuilder.DropTable(
                name: "Orcamentos");

            migrationBuilder.DropTable(
                name: "Transacoes");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "ContasBancarias");
        }
    }
}
