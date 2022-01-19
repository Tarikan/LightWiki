using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class ModifyArticleIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_articles_name",
                table: "articles");

            migrationBuilder.AddColumn<string>(
                name: "article_tree_id",
                table: "workspaces",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_name_workspace_id",
                table: "articles",
                columns: new[] { "name", "workspace_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_articles_name_workspace_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "article_tree_id",
                table: "workspaces");

            migrationBuilder.CreateIndex(
                name: "ix_articles_name",
                table: "articles",
                column: "name",
                unique: true);
        }
    }
}
