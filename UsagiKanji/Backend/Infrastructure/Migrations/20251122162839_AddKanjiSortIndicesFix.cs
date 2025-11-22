using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKanjiSortIndicesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortIndex_Grade",
                table: "Kanji",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortIndex_JLPT",
                table: "Kanji",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortIndex_Grade",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "SortIndex_JLPT",
                table: "Kanji");
        }
    }
}
