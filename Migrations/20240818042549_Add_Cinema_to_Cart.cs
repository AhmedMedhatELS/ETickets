using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETickets.Migrations
{
    /// <inheritdoc />
    public partial class Add_Cinema_to_Cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaId",
                table: "TicketsCart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketsCart_CinemaId",
                table: "TicketsCart",
                column: "CinemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketsCart_Cinema_CinemaId",
                table: "TicketsCart",
                column: "CinemaId",
                principalTable: "Cinema",
                principalColumn: "CinemaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketsCart_Cinema_CinemaId",
                table: "TicketsCart");

            migrationBuilder.DropIndex(
                name: "IX_TicketsCart_CinemaId",
                table: "TicketsCart");

            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "TicketsCart");
        }
    }
}
