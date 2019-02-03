using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensHagen.Models
{

    public class DataBaseContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<UserLogItem> UserLogItems { get; set; }


        public DbSet<Sensor> Sensors { get; set; }

        public DbSet<SensorLogItem> SensorLogItems { get; set; }
                

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/SensHagen.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLogItem>()
                .HasOne(p => p.User)
                .WithMany(b => b.LogItems)
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("ForeignKey_UserLogItem_User");

        }

    }

    public enum SensorLogItemType
    {
        Unknown = 0,
        Heartbeat = 1,
        DetectionOn = 2,
        DetectionOff = 3
    }

    public class Sensor
    {
        public int SensorId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }


        public List<SensorLogItem> LogItems { get; set; }

    }


    public class SensorLogItem
    {

        public int SensorLogItemId { get; set; }
        public SensorLogItemType Type { get; set; }
        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public List<UserLogItem> LogItems { get; set; }

        public List<Sensor> Sensors { get; set; }
    }


    public class UserLogItem
    {
        public int UserLogItemId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LogItemType { get; set; }
        public string Value { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }    
}

