using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgoSM2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EaseFactor",
                table: "UserKanjis",
                type: "float",
                nullable: false,
                defaultValue: 2.5);

            migrationBuilder.AddColumn<int>(
                name: "Lapses",
                table: "UserKanjis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Repetitions",
                table: "UserKanjis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EaseFactor",
                table: "UserKanjis");

            migrationBuilder.DropColumn(
                name: "Lapses",
                table: "UserKanjis");

            migrationBuilder.DropColumn(
                name: "Repetitions",
                table: "UserKanjis");
        }
    }
}
