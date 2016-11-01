﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App2NightAPI.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationItems",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CityName = table.Column<string>(nullable: true),
                    CountryName = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<int>(nullable: false),
                    HouseNumberAdditional = table.Column<string>(nullable: true),
                    Latitude = table.Column<long>(nullable: false),
                    Longitude = table.Column<long>(nullable: false),
                    StreetName = table.Column<string>(nullable: true),
                    Zipcode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationItems", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "UserItems",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItems", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserItems_LocationItems_LocationId",
                        column: x => x.LocationId,
                        principalTable: "LocationItems",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartyItems",
                columns: table => new
                {
                    PartId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    HostUserId = table.Column<Guid>(nullable: true),
                    LocationId = table.Column<int>(nullable: false),
                    MusicGenre = table.Column<int>(nullable: false),
                    PartyDate = table.Column<DateTime>(nullable: false),
                    PartyName = table.Column<string>(nullable: false),
                    PartyType = table.Column<int>(nullable: false),
                    Price = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyItems", x => x.PartId);
                    table.ForeignKey(
                        name: "FK_PartyItems_UserItems_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "UserItems",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartyItems_LocationItems_LocationId",
                        column: x => x.LocationId,
                        principalTable: "LocationItems",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserItems_LocationId",
                table: "UserItems",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyItems_HostUserId",
                table: "PartyItems",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyItems_LocationId",
                table: "PartyItems",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyItems");

            migrationBuilder.DropTable(
                name: "UserItems");

            migrationBuilder.DropTable(
                name: "LocationItems");
        }
    }
}
