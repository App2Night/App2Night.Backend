﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App2NightAPI.Models.Authentification;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace App2NightAPI.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Party> PartyItems { get; set; }
        public DbSet<User> UserItems { get; set; }
        public DbSet<Location> LocationItems { get; set; }
        public DbSet<UserParty> UserPartyItems { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add keys UserId and PartyId to UserParty table
            modelBuilder.Entity<UserParty>().HasKey(x => new { x.UserId, x.PartyId });
            base.OnModelCreating(modelBuilder);

        }
    }
}
