using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKanjis_Kanji_KanjiId",
                table: "UserKanjis");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKanjis_Users_UserId",
                table: "UserKanjis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "UserKanjis",
                newName: "UserKanji");

            migrationBuilder.RenameIndex(
                name: "IX_UserKanjis_KanjiId",
                table: "UserKanji",
                newName: "IX_UserKanji_KanjiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserKanji",
                table: "UserKanji",
                columns: new[] { "UserId", "KanjiId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserKanji_Kanji_KanjiId",
                table: "UserKanji",
                column: "KanjiId",
                principalTable: "Kanji",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKanji_User_UserId",
                table: "UserKanji",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKanji_Kanji_KanjiId",
                table: "UserKanji");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKanji_User_UserId",
                table: "UserKanji");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserKanji",
                table: "UserKanji");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "UserKanji",
                newName: "UserKanjis");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_UserKanji_KanjiId",
                table: "UserKanjis",
                newName: "IX_UserKanjis_KanjiId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis",
                columns: new[] { "UserId", "KanjiId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserKanjis_Kanji_KanjiId",
                table: "UserKanjis",
                column: "KanjiId",
                principalTable: "Kanji",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKanjis_Users_UserId",
                table: "UserKanjis",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
