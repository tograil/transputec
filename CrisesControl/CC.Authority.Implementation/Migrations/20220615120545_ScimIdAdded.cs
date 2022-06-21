using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CC.Authority.Implementation.Migrations
{
    public partial class ScimIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalScimId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalScimId",
                table: "Users");
        }
    }
}
