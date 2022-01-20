using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddArticleParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "article_tree_id",
                table: "workspaces");

            migrationBuilder.AddColumn<int>(
                name: "parent_article_id",
                table: "articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_parent_article_id",
                table: "articles",
                column: "parent_article_id");

            migrationBuilder.AddForeignKey(
                name: "fk_articles_articles_parent_article_id",
                table: "articles",
                column: "parent_article_id",
                principalTable: "articles",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_articles_articles_parent_article_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_parent_article_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "parent_article_id",
                table: "articles");

            migrationBuilder.AddColumn<string>(
                name: "article_tree_id",
                table: "workspaces",
                type: "text",
                nullable: true);
        }
    }
}
