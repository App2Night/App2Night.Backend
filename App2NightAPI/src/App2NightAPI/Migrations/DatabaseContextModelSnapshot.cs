using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App2NightAPI.Models;

namespace App2NightAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("App2NightAPI.Models.Authentification.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("UserId");

                    b.ToTable("UserItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.Property<Guid>("PartId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<Guid?>("HostUserId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("PartId");

                    b.HasIndex("HostUserId");

                    b.ToTable("PartyItems");
                });

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.HasOne("App2NightAPI.Models.Authentification.User", "Host")
                        .WithMany("PartyHostedByUser")
                        .HasForeignKey("HostUserId");
                });
        }
    }
}
