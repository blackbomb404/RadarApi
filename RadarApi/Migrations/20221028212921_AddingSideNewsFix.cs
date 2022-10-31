using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadarApi.Migrations
{
    public partial class AddingSideNewsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "SideNews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "SideNews");
        }
    }
}
