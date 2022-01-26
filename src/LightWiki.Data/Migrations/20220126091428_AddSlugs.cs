using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddSlugs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "workspaces",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "groups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "articles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_workspaces_slug",
                table: "workspaces",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_groups_slug",
                table: "groups",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_slug_workspace_id",
                table: "articles",
                columns: new[] { "slug", "workspace_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_workspaces_slug",
                table: "workspaces");

            migrationBuilder.DropIndex(
                name: "ix_groups_slug",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_articles_slug_workspace_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "workspaces");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "articles");
        }
    }
}
