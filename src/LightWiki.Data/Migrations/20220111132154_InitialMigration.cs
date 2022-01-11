using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GROUPS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NAME = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GROUPS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NAME = table.Column<string>(type: "text", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NAME = table.Column<string>(type: "text", nullable: true),
                    USER_ID = table.Column<int>(type: "integer", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GLOBAL_ACCESS_RULE = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARTICLES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GROUP_USER",
                columns: table => new
                {
                    GROUPS_ID = table.Column<int>(type: "integer", nullable: false),
                    USERS_ID = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GROUP_USER", x => new { x.GROUPS_ID, x.USERS_ID });
                    table.ForeignKey(
                        name: "FK_GROUP_USER_GROUPS_GROUPS_ID",
                        column: x => x.GROUPS_ID,
                        principalTable: "GROUPS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GROUP_USER_USERS_USERS_ID",
                        column: x => x.USERS_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLE_GROUP_ACCESS_RULES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GROUP_ID = table.Column<int>(type: "integer", nullable: false),
                    ARTICLE_ID = table.Column<int>(type: "integer", nullable: false),
                    ARTICLE_ACCESS_RULE = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_GROUP_ACCESS_RULES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARTICLE_GROUP_ACCESS_RULES_ARTICLES_ARTICLE_ID",
                        column: x => x.ARTICLE_ID,
                        principalTable: "ARTICLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARTICLE_GROUP_ACCESS_RULES_GROUPS_GROUP_ID",
                        column: x => x.GROUP_ID,
                        principalTable: "GROUPS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLE_PERSONAL_ACCESS_RULES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USER_ID = table.Column<int>(type: "integer", nullable: false),
                    ARTICLE_ID = table.Column<int>(type: "integer", nullable: false),
                    ARTICLE_ACCESS_RULE = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_PERSONAL_ACCESS_RULES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARTICLE_PERSONAL_ACCESS_RULES_ARTICLES_ARTICLE_ID",
                        column: x => x.ARTICLE_ID,
                        principalTable: "ARTICLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARTICLE_PERSONAL_ACCESS_RULES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARTICLE_VERSIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    USER_ID = table.Column<int>(type: "integer", nullable: false),
                    PATCH = table.Column<string>(type: "text", nullable: true),
                    ARTICLE_ID = table.Column<int>(type: "integer", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_VERSIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ARTICLE_VERSIONS_ARTICLES_ARTICLE_ID",
                        column: x => x.ARTICLE_ID,
                        principalTable: "ARTICLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARTICLE_VERSIONS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_GROUP_ACCESS_RULES_ARTICLE_ID",
                table: "ARTICLE_GROUP_ACCESS_RULES",
                column: "ARTICLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_GROUP_ACCESS_RULES_GROUP_ID",
                table: "ARTICLE_GROUP_ACCESS_RULES",
                column: "GROUP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_PERSONAL_ACCESS_RULES_ARTICLE_ID",
                table: "ARTICLE_PERSONAL_ACCESS_RULES",
                column: "ARTICLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_PERSONAL_ACCESS_RULES_USER_ID",
                table: "ARTICLE_PERSONAL_ACCESS_RULES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_VERSIONS_ARTICLE_ID",
                table: "ARTICLE_VERSIONS",
                column: "ARTICLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_VERSIONS_USER_ID",
                table: "ARTICLE_VERSIONS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLES_USER_ID",
                table: "ARTICLES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_GROUP_USER_USERS_ID",
                table: "GROUP_USER",
                column: "USERS_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARTICLE_GROUP_ACCESS_RULES");

            migrationBuilder.DropTable(
                name: "ARTICLE_PERSONAL_ACCESS_RULES");

            migrationBuilder.DropTable(
                name: "ARTICLE_VERSIONS");

            migrationBuilder.DropTable(
                name: "GROUP_USER");

            migrationBuilder.DropTable(
                name: "ARTICLES");

            migrationBuilder.DropTable(
                name: "GROUPS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
