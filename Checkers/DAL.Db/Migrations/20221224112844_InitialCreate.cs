using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckersOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GameBoardHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    GameBoardWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    WhitePieces = table.Column<int>(type: "INTEGER", nullable: false),
                    BlackPieces = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckersGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameOverAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GameWonByPlayer = table.Column<string>(type: "TEXT", nullable: true),
                    WhiteName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    WhitePieces = table.Column<int>(type: "INTEGER", nullable: false),
                    BlackName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    BlackPieces = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckerOptionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersGame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckersGame_CheckersOption_CheckerOptionId",
                        column: x => x.CheckerOptionId,
                        principalTable: "CheckersOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckersGameState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SerializedGameState = table.Column<string>(type: "TEXT", nullable: false),
                    CheckersGameId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckersGameState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckersGameState_CheckersGame_CheckersGameId",
                        column: x => x.CheckersGameId,
                        principalTable: "CheckersGame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckersGame_CheckerOptionId",
                table: "CheckersGame",
                column: "CheckerOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckersGameState_CheckersGameId",
                table: "CheckersGameState",
                column: "CheckersGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckersGameState");

            migrationBuilder.DropTable(
                name: "CheckersGame");

            migrationBuilder.DropTable(
                name: "CheckersOption");
        }
    }
}
