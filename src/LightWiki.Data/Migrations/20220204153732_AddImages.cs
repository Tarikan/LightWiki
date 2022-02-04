using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LightWiki.Data.Migrations
{
    public partial class AddImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<int>(type: "integer", nullable: false),
                    owner_type = table.Column<int>(type: "integer", nullable: false),
                    folder = table.Column<string>(type: "text", nullable: true),
                    file_name = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_images_owner_id",
                table: "images",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_images_owner_type",
                table: "images",
                column: "owner_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "images");
        }
    }
}
