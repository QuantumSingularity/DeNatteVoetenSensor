using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvs.Models.SqLite
{

    public class DataBaseContext : DbContext
    {


        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }


        public DbSet<User> Users { get; set; }

        public DbSet<UserLogItem> UserLogItems { get; set; }


        public DbSet<Sensor> Sensors { get; set; }

        public DbSet<SensorLogItem> SensorLogItems { get; set; }
                

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLogItem>()
                .HasOne(p => p.User)
                .WithMany(b => b.LogItems)
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("ForeignKey_UserLogItem_User");

            modelBuilder.Entity<Sensor>()
                .HasAlternateKey(c => c.MacAddress)
                .HasName("AlternateKey_MacAddress");                

        }

    }

}

