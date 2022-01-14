using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class RemovePublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "public_id",
                table: "users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "public_id",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
