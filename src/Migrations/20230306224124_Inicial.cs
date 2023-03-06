using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Loteria.API.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LOTERIA",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", maxLength: 5, nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LOTERIA = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    CONCURSO = table.Column<int>(type: "int", maxLength: 8, nullable: false),
                    RESULTADO = table.Column<string>(type: "longtext", nullable: false),
                    DEZENA1 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DEZENA2 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DEZENA3 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DEZENA4 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DEZENA5 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DEZENA6 = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DATA_PROXIMO_CONCURSO = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ID", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LOTERIA_CONCURSO_LOTERIA",
                table: "LOTERIA",
                columns: new[] { "CONCURSO", "LOTERIA" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOTERIA");
        }
    }
}
