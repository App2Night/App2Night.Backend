using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace App2NightAPI.Migrations
{
    public partial class TestMigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartyItems",
                columns: table => new
                {
                    PartId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyItems", x => x.PartId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyItems");
        }
    }
}
