using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class ReplaceAccessSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_article_group_access_rules_articles_article_id",
                table: "article_group_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_article_group_access_rules_groups_group_id",
                table: "article_group_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_article_personal_access_rules_articles_article_id",
                table: "article_personal_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_article_personal_access_rules_users_user_id",
                table: "article_personal_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_group_access_rules_groups_group_id",
                table: "workspace_group_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_group_access_rules_workspaces_workspace_id",
                table: "workspace_group_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_personal_access_rules_users_user_id",
                table: "workspace_personal_access_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_personal_access_rules_workspaces_workspace_id",
                table: "workspace_personal_access_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_workspace_personal_access_rules",
                table: "workspace_personal_access_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_workspace_group_access_rules",
                table: "workspace_group_access_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_article_personal_access_rules",
                table: "article_personal_access_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_article_group_access_rules",
                table: "article_group_access_rules");

            migrationBuilder.DropColumn(
                name: "workspace_access_rule",
                table: "workspaces");

            migrationBuilder.DropColumn(
                name: "global_access_rule",
                table: "articles");

            migrationBuilder.RenameTable(
                name: "workspace_personal_access_rules",
                newName: "workspace_personal_access_rule");

            migrationBuilder.RenameTable(
                name: "workspace_group_access_rules",
                newName: "workspace_group_access_rule");

            migrationBuilder.RenameTable(
                name: "article_personal_access_rules",
                newName: "article_personal_access_rule");

            migrationBuilder.RenameTable(
                name: "article_group_access_rules",
                newName: "article_group_access_rule");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_personal_access_rules_workspace_id",
                table: "workspace_personal_access_rule",
                newName: "ix_workspace_personal_access_rule_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_personal_access_rules_user_id_workspace_id",
                table: "workspace_personal_access_rule",
                newName: "ix_workspace_personal_access_rule_user_id_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_group_access_rules_workspace_id",
                table: "workspace_group_access_rule",
                newName: "ix_workspace_group_access_rule_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_group_access_rules_group_id_workspace_id",
                table: "workspace_group_access_rule",
                newName: "ix_workspace_group_access_rule_group_id_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_personal_access_rules_user_id_article_id",
                table: "article_personal_access_rule",
                newName: "ix_article_personal_access_rule_user_id_article_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_personal_access_rules_article_id",
                table: "article_personal_access_rule",
                newName: "ix_article_personal_access_rule_article_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_group_access_rules_group_id",
                table: "article_group_access_rule",
                newName: "ix_article_group_access_rule_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_group_access_rules_article_id_group_id",
                table: "article_group_access_rule",
                newName: "ix_article_group_access_rule_article_id_group_id");

            migrationBuilder.AddColumn<int>(
                name: "party_id",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "party_id",
                table: "groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_workspace_personal_access_rule",
                table: "workspace_personal_access_rule",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_workspace_group_access_rule",
                table: "workspace_group_access_rule",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_article_personal_access_rule",
                table: "article_personal_access_rule",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_article_group_access_rule",
                table: "article_group_access_rule",
                column: "id");

            migrationBuilder.CreateTable(
                name: "parties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_type = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "article_accesses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_id = table.Column<int>(type: "integer", nullable: false),
                    article_access_rule = table.Column<int>(type: "integer", nullable: false),
                    article_id = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_accesses", x => x.id);
                    table.ForeignKey(
                        name: "fk_article_accesses_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_article_accesses_parties_party_id",
                        column: x => x.party_id,
                        principalTable: "parties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workspace_accesses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workspace_id = table.Column<int>(type: "integer", nullable: false),
                    party_id = table.Column<int>(type: "integer", nullable: false),
                    workspace_access_rule = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_accesses", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_accesses_parties_party_id",
                        column: x => x.party_id,
                        principalTable: "parties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_workspace_accesses_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_users_party_id",
                table: "users",
                column: "party_id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_party_id",
                table: "groups",
                column: "party_id");

            migrationBuilder.CreateIndex(
                name: "ix_article_accesses_article_id",
                table: "article_accesses",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "ix_article_accesses_party_id_article_id",
                table: "article_accesses",
                columns: new[] { "party_id", "article_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_parties_id_party_type",
                table: "parties",
                columns: new[] { "id", "party_type" });

            migrationBuilder.CreateIndex(
                name: "ix_workspace_accesses_party_id_workspace_id",
                table: "workspace_accesses",
                columns: new[] { "party_id", "workspace_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_workspace_accesses_workspace_id",
                table: "workspace_accesses",
                column: "workspace_id");

            migrationBuilder.AddForeignKey(
                name: "fk_article_group_access_rule_articles_article_id",
                table: "article_group_access_rule",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_group_access_rule_groups_group_id",
                table: "article_group_access_rule",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_personal_access_rule_articles_article_id",
                table: "article_personal_access_rule",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_personal_access_rule_users_user_id",
                table: "article_personal_access_rule",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_groups_parties_party_id",
                table: "groups",
                column: "party_id",
                principalTable: "parties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_parties_party_id",
                table: "users",
                column: "party_id",
                principalTable: "parties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_group_access_rule_groups_group_id",
                table: "workspace_group_access_rule",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_group_access_rule_workspaces_workspace_id",
                table: "workspace_group_access_rule",
                column: "workspace_id",
                principalTable: "workspaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_personal_access_rule_users_user_id",
                table: "workspace_personal_access_rule",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_personal_access_rule_workspaces_workspace_id",
                table: "workspace_personal_access_rule",
                column: "workspace_id",
                principalTable: "workspaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_article_group_access_rule_articles_article_id",
                table: "article_group_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_article_group_access_rule_groups_group_id",
                table: "article_group_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_article_personal_access_rule_articles_article_id",
                table: "article_personal_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_article_personal_access_rule_users_user_id",
                table: "article_personal_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_groups_parties_party_id",
                table: "groups");

            migrationBuilder.DropForeignKey(
                name: "fk_users_parties_party_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_group_access_rule_groups_group_id",
                table: "workspace_group_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_group_access_rule_workspaces_workspace_id",
                table: "workspace_group_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_personal_access_rule_users_user_id",
                table: "workspace_personal_access_rule");

            migrationBuilder.DropForeignKey(
                name: "fk_workspace_personal_access_rule_workspaces_workspace_id",
                table: "workspace_personal_access_rule");

            migrationBuilder.DropTable(
                name: "article_accesses");

            migrationBuilder.DropTable(
                name: "workspace_accesses");

            migrationBuilder.DropTable(
                name: "parties");

            migrationBuilder.DropIndex(
                name: "ix_users_party_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_groups_party_id",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "pk_workspace_personal_access_rule",
                table: "workspace_personal_access_rule");

            migrationBuilder.DropPrimaryKey(
                name: "pk_workspace_group_access_rule",
                table: "workspace_group_access_rule");

            migrationBuilder.DropPrimaryKey(
                name: "pk_article_personal_access_rule",
                table: "article_personal_access_rule");

            migrationBuilder.DropPrimaryKey(
                name: "pk_article_group_access_rule",
                table: "article_group_access_rule");

            migrationBuilder.DropColumn(
                name: "party_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "party_id",
                table: "groups");

            migrationBuilder.RenameTable(
                name: "workspace_personal_access_rule",
                newName: "workspace_personal_access_rules");

            migrationBuilder.RenameTable(
                name: "workspace_group_access_rule",
                newName: "workspace_group_access_rules");

            migrationBuilder.RenameTable(
                name: "article_personal_access_rule",
                newName: "article_personal_access_rules");

            migrationBuilder.RenameTable(
                name: "article_group_access_rule",
                newName: "article_group_access_rules");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_personal_access_rule_workspace_id",
                table: "workspace_personal_access_rules",
                newName: "ix_workspace_personal_access_rules_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_personal_access_rule_user_id_workspace_id",
                table: "workspace_personal_access_rules",
                newName: "ix_workspace_personal_access_rules_user_id_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_group_access_rule_workspace_id",
                table: "workspace_group_access_rules",
                newName: "ix_workspace_group_access_rules_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_workspace_group_access_rule_group_id_workspace_id",
                table: "workspace_group_access_rules",
                newName: "ix_workspace_group_access_rules_group_id_workspace_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_personal_access_rule_user_id_article_id",
                table: "article_personal_access_rules",
                newName: "ix_article_personal_access_rules_user_id_article_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_personal_access_rule_article_id",
                table: "article_personal_access_rules",
                newName: "ix_article_personal_access_rules_article_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_group_access_rule_group_id",
                table: "article_group_access_rules",
                newName: "ix_article_group_access_rules_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_article_group_access_rule_article_id_group_id",
                table: "article_group_access_rules",
                newName: "ix_article_group_access_rules_article_id_group_id");

            migrationBuilder.AddColumn<int>(
                name: "workspace_access_rule",
                table: "workspaces",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "global_access_rule",
                table: "articles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_workspace_personal_access_rules",
                table: "workspace_personal_access_rules",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_workspace_group_access_rules",
                table: "workspace_group_access_rules",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_article_personal_access_rules",
                table: "article_personal_access_rules",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_article_group_access_rules",
                table: "article_group_access_rules",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_article_group_access_rules_articles_article_id",
                table: "article_group_access_rules",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_group_access_rules_groups_group_id",
                table: "article_group_access_rules",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_personal_access_rules_articles_article_id",
                table: "article_personal_access_rules",
                column: "article_id",
                principalTable: "articles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_article_personal_access_rules_users_user_id",
                table: "article_personal_access_rules",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_group_access_rules_groups_group_id",
                table: "workspace_group_access_rules",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_group_access_rules_workspaces_workspace_id",
                table: "workspace_group_access_rules",
                column: "workspace_id",
                principalTable: "workspaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_personal_access_rules_users_user_id",
                table: "workspace_personal_access_rules",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_workspace_personal_access_rules_workspaces_workspace_id",
                table: "workspace_personal_access_rules",
                column: "workspace_id",
                principalTable: "workspaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
