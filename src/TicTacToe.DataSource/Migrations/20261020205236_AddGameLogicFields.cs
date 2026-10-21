using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class AddGameLogicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "board",
                table: "games");

            migrationBuilder.AddColumn<Guid>(
                name: "currentPlayerId",
                table: "games",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "player1Symbol",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "player2Symbol",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "winnerId",
                table: "games",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currentPlayerId",
                table: "games");

            migrationBuilder.DropColumn(
                name: "player1Symbol",
                table: "games");

            migrationBuilder.DropColumn(
                name: "player2Symbol",
                table: "games");

            migrationBuilder.DropColumn(
                name: "winnerId",
                table: "games");

            migrationBuilder.AddColumn<string>(
                name: "board",
                table: "games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
