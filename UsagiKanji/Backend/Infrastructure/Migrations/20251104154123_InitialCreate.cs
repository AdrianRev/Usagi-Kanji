using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kanji",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Character = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StrokeCount = table.Column<int>(type: "int", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    JLPTLevel = table.Column<int>(type: "int", nullable: true),
                    FrequencyRank = table.Column<int>(type: "int", nullable: true),
                    HeisigNumber = table.Column<int>(type: "int", nullable: true),
                    Heisig6Number = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kanji", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vocabulary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JMdictId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabulary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meaning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meaning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meaning_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reading",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reading_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyGloss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    VocabularyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyGloss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyGloss_Vocabulary_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyKana",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Common = table.Column<bool>(type: "bit", nullable: false),
                    AppliesToKanji = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VocabularyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyKana", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyKana_Vocabulary_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyKanjiForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Common = table.Column<bool>(type: "bit", nullable: false),
                    VocabularyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyKanjiForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyKanjiForm_Vocabulary_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyKanjiCharacter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VocabularyKanjiFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyKanjiCharacter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyKanjiCharacter_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VocabularyKanjiCharacter_VocabularyKanjiForm_VocabularyKanjiFormId",
                        column: x => x.VocabularyKanjiFormId,
                        principalTable: "VocabularyKanjiForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meaning_KanjiId",
                table: "Meaning",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_Reading_KanjiId",
                table: "Reading",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyGloss_VocabularyId",
                table: "VocabularyGloss",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyKana_VocabularyId",
                table: "VocabularyKana",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyKanjiCharacter_KanjiId",
                table: "VocabularyKanjiCharacter",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyKanjiCharacter_VocabularyKanjiFormId",
                table: "VocabularyKanjiCharacter",
                column: "VocabularyKanjiFormId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyKanjiForm_VocabularyId",
                table: "VocabularyKanjiForm",
                column: "VocabularyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meaning");

            migrationBuilder.DropTable(
                name: "Reading");

            migrationBuilder.DropTable(
                name: "VocabularyGloss");

            migrationBuilder.DropTable(
                name: "VocabularyKana");

            migrationBuilder.DropTable(
                name: "VocabularyKanjiCharacter");

            migrationBuilder.DropTable(
                name: "Kanji");

            migrationBuilder.DropTable(
                name: "VocabularyKanjiForm");

            migrationBuilder.DropTable(
                name: "Vocabulary");
        }
    }
}
