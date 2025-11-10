using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserKanjiCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis");

            migrationBuilder.DropIndex(
                name: "IX_UserKanjis_UserId",
                table: "UserKanjis");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserKanjis");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis",
                columns: new[] { "UserId", "KanjiId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserKanjis",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserKanjis",
                table: "UserKanjis",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserKanjis_UserId",
                table: "UserKanjis",
                column: "UserId");
        }
    }
}
