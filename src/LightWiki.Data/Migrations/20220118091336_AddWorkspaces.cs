using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddWorkspaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "workspace_id",
                table: "articles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "workspaces",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    workspace_access_rule = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspaces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_group_access_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    workspace_access_rule = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_group_access_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_group_access_rules_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workspace_group_access_rules_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_personal_access_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    workspace_access_rule = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_personal_access_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_personal_access_rules_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workspace_personal_access_rules_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_articles_workspace_id",
                table: "articles",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_group_access_rules_group_id_workspace_id",
                table: "workspace_group_access_rules",
                columns: new[] { "group_id", "workspace_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workspace_group_access_rules_workspace_id",
                table: "workspace_group_access_rules",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_personal_access_rules_user_id_workspace_id",
                table: "workspace_personal_access_rules",
                columns: new[] { "user_id", "workspace_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workspace_personal_access_rules_workspace_id",
                table: "workspace_personal_access_rules",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspaces_name",
                table: "workspaces",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_articles_workspaces_workspace_id",
                table: "articles",
                column: "workspace_id",
                principalTable: "workspaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_articles_workspaces_workspace_id",
                table: "articles");

            migrationBuilder.DropTable(
                name: "workspace_group_access_rules");

            migrationBuilder.DropTable(
                name: "workspace_personal_access_rules");

            migrationBuilder.DropTable(
                name: "workspaces");

            migrationBuilder.DropIndex(
                name: "ix_articles_workspace_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                table: "articles");
        }
    }
}
