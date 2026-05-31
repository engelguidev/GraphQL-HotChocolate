using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "Tickets",
                type: "int",
                nullable: true,
                defaultValue: null);

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DepartamentoId",
                table: "Tickets",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Nome",
                table: "Departamentos",
                column: "Nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Departamentos_DepartamentoId",
                table: "Tickets",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Departamentos_DepartamentoId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DepartamentoId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Tickets");
        }
    }
}
