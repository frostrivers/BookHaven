using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHaven.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCardImageToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardImage",
                table: "Events",
                type: "LONGTEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardImage",
                table: "Events");
        }
    }
}
