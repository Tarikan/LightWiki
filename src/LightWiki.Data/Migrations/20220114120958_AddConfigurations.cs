using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_article_personal_access_rules_user_id",
                table: "article_personal_access_rules");

            migrationBuilder.DropIndex(
                name: "ix_article_group_access_rules_article_id",
                table: "article_group_access_rules");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_name",
                table: "users",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_groups_name",
                table: "groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_article_personal_access_rules_user_id_article_id",
                table: "article_personal_access_rules",
                columns: new[] { "user_id", "article_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_article_group_access_rules_article_id_group_id",
                table: "article_group_access_rules",
                columns: new[] { "article_id", "group_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_name",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_groups_name",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_article_personal_access_rules_user_id_article_id",
                table: "article_personal_access_rules");

            migrationBuilder.DropIndex(
                name: "ix_article_group_access_rules_article_id_group_id",
                table: "article_group_access_rules");

            migrationBuilder.CreateIndex(
                name: "ix_article_personal_access_rules_user_id",
                table: "article_personal_access_rules",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_article_group_access_rules_article_id",
                table: "article_group_access_rules",
                column: "article_id");
        }
    }
}
