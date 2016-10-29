using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App2NightAPI.Models.Authentification;
namespace App2NightAPI.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Party> PartyItems { get; set; }
        //public DbSet<User> UserItems { get; set; }
        public DbSet<Location> LocationItems { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Relationen zwischen DBs festlegen
            //Suchen nach ASPNETCORE Entity Framework

        }
    }
}
