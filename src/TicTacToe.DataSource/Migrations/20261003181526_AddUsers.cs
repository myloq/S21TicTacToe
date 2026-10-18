using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
