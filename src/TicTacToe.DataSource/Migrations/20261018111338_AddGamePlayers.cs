using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class AddGamePlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "player1",
                table: "games",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "player2",
                table: "games",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "player1",
                table: "games");

            migrationBuilder.DropColumn(
                name: "player2",
                table: "games");
        }
    }
}
