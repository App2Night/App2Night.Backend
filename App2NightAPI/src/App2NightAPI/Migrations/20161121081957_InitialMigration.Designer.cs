using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App2NightAPI.Models;

namespace App2NightAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20161121081957_InitialMigration")]
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

                    b.Property<string>("HouseNumber");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("StreetName");

                    b.Property<string>("Zipcode");

                    b.HasKey("LocationId");

                    b.ToTable("LocationItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.Property<Guid>("PartyId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<Guid?>("HostUserId");

                    b.Property<int?>("LocationId")
                        .IsRequired();

                    b.Property<int>("MusicGenre");

                    b.Property<DateTime>("PartyDate");

                    b.Property<string>("PartyName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 32);

                    b.Property<int>("PartyType");

                    b.Property<int>("Price");

                    b.HasKey("PartyId");

                    b.HasIndex("HostUserId");

                    b.HasIndex("LocationId");

                    b.ToTable("PartyItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.UserParty", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("PartyId");

                    b.Property<int>("EventCommitment");

                    b.Property<int>("GeneralRating");

                    b.Property<int>("LocationRating");

                    b.Property<int>("MoodRating");

                    b.Property<int>("PriceRating");

                    b.HasKey("UserId", "PartyId");

                    b.HasIndex("PartyId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPartyItems");
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

            modelBuilder.Entity("App2NightAPI.Models.UserParty", b =>
                {
                    b.HasOne("App2NightAPI.Models.Party", "Party")
                        .WithMany()
                        .HasForeignKey("PartyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App2NightAPI.Models.Authentification.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
