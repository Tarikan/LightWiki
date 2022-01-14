using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class MakeArticleNameUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_articles_name",
                table: "articles",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_articles_name",
                table: "articles");
        }
    }
}
