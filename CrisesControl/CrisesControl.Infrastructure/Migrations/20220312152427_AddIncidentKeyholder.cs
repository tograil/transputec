using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrisesControl.Infrastructure.Migrations
{
    public partial class AddIncidentKeyholder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentKeyholder",
                columns: table => new
                {
                    IncidentKeyholderID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentID = table.Column<int>(type: "int", nullable: true),
                    ActiveIncidentID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    CompanyID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentKeyholder", x => x.IncidentKeyholderID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentKeyholder");
        }
    }
}
