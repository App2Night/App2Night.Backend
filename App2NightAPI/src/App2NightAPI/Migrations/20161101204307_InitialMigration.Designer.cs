﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App2NightAPI.Models;

namespace App2NightAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20161101204307_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("App2NightAPI.Models.Authentification.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<int?>("LocationId");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("UserId");

                    b.HasIndex("LocationId");

                    b.ToTable("UserItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CityName");

                    b.Property<string>("CountryName");

                    b.Property<int>("HouseNumber");

                    b.Property<string>("HouseNumberAdditional");

                    b.Property<long>("Latitude");

                    b.Property<long>("Longitude");

                    b.Property<string>("StreetName");

                    b.Property<int>("Zipcode");

                    b.HasKey("LocationId");

                    b.ToTable("LocationItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.Property<Guid>("PartId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<Guid?>("HostUserId");

                    b.Property<int?>("LocationId")
                        .IsRequired();

                    b.Property<int>("MusicGenre");

                    b.Property<DateTime>("PartyDate");

                    b.Property<string>("PartyName")
                        .IsRequired();

                    b.Property<int>("PartyType");

                    b.Property<int>("Price");

                    b.HasKey("PartId");

                    b.HasIndex("HostUserId");

                    b.HasIndex("LocationId");

                    b.ToTable("PartyItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Authentification.User", b =>
                {
                    b.HasOne("App2NightAPI.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.HasOne("App2NightAPI.Models.Authentification.User", "Host")
                        .WithMany("PartyHostedByUser")
                        .HasForeignKey("HostUserId");

                    b.HasOne("App2NightAPI.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}