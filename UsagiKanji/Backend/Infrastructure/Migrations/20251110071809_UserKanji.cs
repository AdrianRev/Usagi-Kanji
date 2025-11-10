using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserKanji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserKanjis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EaseFactor = table.Column<double>(type: "float", nullable: false, defaultValue: 2.5),
                    Interval = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKanjis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserKanjis_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserKanjis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserKanjis_KanjiId",
                table: "UserKanjis",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKanjis_UserId",
                table: "UserKanjis",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserKanjis");
        }
    }
}
