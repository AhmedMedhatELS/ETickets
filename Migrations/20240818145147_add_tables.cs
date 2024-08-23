using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETickets.Migrations
{
    /// <inheritdoc />
    public partial class add_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketsCart",
                columns: table => new
                {
                    TicketsCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketsStatus = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketsCart", x => x.TicketsCartId);
                    table.ForeignKey(
                        name: "FK_TicketsCart_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketsCart_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketsCinemas",
                columns: table => new
                {
                    TicketsCinemaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QNumber = table.Column<int>(type: "int", nullable: false),
                    TicketsCartId = table.Column<int>(type: "int", nullable: false),
                    CinemaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketsCinemas", x => x.TicketsCinemaId);
                    table.ForeignKey(
                        name: "FK_TicketsCinemas_Cinema_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinema",
                        principalColumn: "CinemaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketsCinemas_TicketsCart_TicketsCartId",
                        column: x => x.TicketsCartId,
                        principalTable: "TicketsCart",
                        principalColumn: "TicketsCartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketsCart_MovieId",
                table: "TicketsCart",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketsCart_UserId",
                table: "TicketsCart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketsCinemas_CinemaId",
                table: "TicketsCinemas",
                column: "CinemaId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketsCinemas_TicketsCartId",
                table: "TicketsCinemas",
                column: "TicketsCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketsCinemas");

            migrationBuilder.DropTable(
                name: "TicketsCart");
        }
    }
}
