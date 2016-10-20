using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App2NightAPI.Models;

namespace App2NightAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20161017120429_TestMigration1")]
    partial class TestMigration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("App2NightAPI.Models.Party", b =>
                {
                    b.Property<Guid>("PartId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("PartId");

                    b.ToTable("PartyItems");
                });
        }
    }
}
