using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App2NightAPI.Migrations
{
    public partial class TestMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserItems",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItems", x => x.UserId);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "HostUserId",
                table: "PartyItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyItems_HostUserId",
                table: "PartyItems",
                column: "HostUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartyItems_UserItems_HostUserId",
                table: "PartyItems",
                column: "HostUserId",
                principalTable: "UserItems",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyItems_UserItems_HostUserId",
                table: "PartyItems");

            migrationBuilder.DropIndex(
                name: "IX_PartyItems_HostUserId",
                table: "PartyItems");

            migrationBuilder.DropColumn(
                name: "HostUserId",
                table: "PartyItems");

            migrationBuilder.DropTable(
                name: "UserItems");
        }
    }
}
