using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddRootArticleToWorkspace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "root_article_id",
                table: "workspaces",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_workspaces_root_article_id",
                table: "workspaces",
                column: "root_article_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_workspaces_articles_root_article_id1",
                table: "workspaces",
                column: "root_article_id",
                principalTable: "articles",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workspaces_articles_root_article_id1",
                table: "workspaces");

            migrationBuilder.DropIndex(
                name: "ix_workspaces_root_article_id",
                table: "workspaces");

            migrationBuilder.DropColumn(
                name: "root_article_id",
                table: "workspaces");
        }
    }
}
