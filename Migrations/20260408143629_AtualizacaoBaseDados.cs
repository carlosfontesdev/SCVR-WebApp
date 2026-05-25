using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _01.Carlos.Fontes.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoBaseDados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.RenameColumn(
                name: "EncomendaId",
                table: "Produtos",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Encomendas",
                newName: "EstadoPagamento");

            migrationBuilder.RenameColumn(
                name: "pts",
                table: "Classificacaos",
                newName: "Pts");

            migrationBuilder.RenameColumn(
                name: "posicao",
                table: "Classificacaos",
                newName: "Posicao");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Classificacaos",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "jogos",
                table: "Classificacaos",
                newName: "Jogos");

            migrationBuilder.RenameColumn(
                name: "Preco",
                table: "Bilhetes",
                newName: "PrecoPago");

            migrationBuilder.RenameColumn(
                name: "DataHora",
                table: "Bilhetes",
                newName: "DataCompra");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Bilhetes",
                newName: "AspNetUserId");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Encomendas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "Encomendas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "JogoId",
                table: "Bilhetes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSocio",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumeroSocio",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "idade",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Encomenda_Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoVenda = table.Column<int>(type: "int", nullable: false),
                    EncomendaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encomenda_Produto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adversario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataJogo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrecoBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LotacaoMaxima = table.Column<int>(type: "int", nullable: false),
                    BilhetesDisponiveis = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Socio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroSocio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socio", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Encomenda_Produto");

            migrationBuilder.DropTable(
                name: "Jogo");

            migrationBuilder.DropTable(
                name: "Socio");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Encomendas");

            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "Encomendas");

            migrationBuilder.DropColumn(
                name: "JogoId",
                table: "Bilhetes");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsSocio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumeroSocio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "idade",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Produtos",
                newName: "EncomendaId");

            migrationBuilder.RenameColumn(
                name: "EstadoPagamento",
                table: "Encomendas",
                newName: "ClienteId");

            migrationBuilder.RenameColumn(
                name: "Pts",
                table: "Classificacaos",
                newName: "pts");

            migrationBuilder.RenameColumn(
                name: "Posicao",
                table: "Classificacaos",
                newName: "posicao");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Classificacaos",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Jogos",
                table: "Classificacaos",
                newName: "jogos");

            migrationBuilder.RenameColumn(
                name: "PrecoPago",
                table: "Bilhetes",
                newName: "Preco");

            migrationBuilder.RenameColumn(
                name: "DataCompra",
                table: "Bilhetes",
                newName: "DataHora");

            migrationBuilder.RenameColumn(
                name: "AspNetUserId",
                table: "Bilhetes",
                newName: "ClienteId");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Idade = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroSocio = table.Column<int>(type: "int", nullable: false),
                    Socio = table.Column<bool>(type: "bit", nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilizadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });
        }
    }
}
