using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensHagen.Models
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
                

        /* 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/SensHagen.db");
        }
        */

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

    public enum SensorLogItemType
    {
        Unknown = 0,
        Heartbeat = 1,
        DetectionOn = 2,
        DetectionOff = 3
    }

    public class Sensor
    {
        public Sensor()
        {
            LogItems = new List<SensorLogItem>();
            RegisterDate = DateTime.Now;
        }

        public int SensorId { get; set; }

        // Unique / AlternateKey
        public string MacAddress { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }

        public DateTime RegisterDate {get; set; }
        public DateTime? ReRegisterDate {get; set; }

        public User User {get; set;}

        public List<SensorLogItem> LogItems { get; set; }

    }


    public class SensorLogItem
    {

        public SensorLogItem()
        {
            this.TimeStamp = DateTime.Now;
        }

        public int SensorLogItemId { get; set; }
        public int SensorId { get; set; }

        public SensorLogItemType LogType { get; set; }
        public string Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public double? BatteryVoltage { get; set; }

        public Sensor Sensor { get; set; }
    }

    public class User
    {
        public User()
        {
            this.CreatedDate = DateTime.Now;
            this.LastModifiedDate = DateTime.Now;
            this.UniqueIdentifier = Guid.NewGuid().ToString();

            LogItems = new List<UserLogItem>();
            Sensors = new List<Sensor>();

        }

        public int UserId { get; set; }
        public string UniqueIdentifier {get;set;}

        public string Name { get; set; }

        [NotMapped]
        public string IpAddress {get; set; }


        [Column]
        public string Password { get; private set; }

        public string EmailAddress { get; set; }

        public List<UserLogItem> LogItems { get; set; }

        public List<Sensor> Sensors { get; set; }



        public DateTime? LastLogOnDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime PasswordChangedDate { get; set; }



        public bool IsDisabled { get; set; }
        public DateTime? DisabledDate { get; set; }



        public bool IsPasswordCorrect(string password)
        {
            return ( this.Password == GenerateHash(password) );
        }

        public void SetPassword(string password)
        {
            Password = GenerateHash(password);
            this.LastModifiedDate = DateTime.Now;
            this.PasswordChangedDate = DateTime.Now;
        }

        private string GenerateHash(string input)
        {
            string result = "";

            // SHA256 is disposable by inheritance.  
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {

                // Add Salt
                input = $"@TeamNVS@-{this.UserId}-{input}-#TeamNVS#";

                // Send a sample text to hash.  
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   
                result = hash;
            }

            return result;
        }

    }


    public class UserLogItem
    {

        public UserLogItem()
        {
            this.TimeStamp = DateTime.Now;
        }

        public int UserLogItemId { get; set; }
        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }
        public string LogItemType { get; set; }
        public string Value { get; set; }

        public User User { get; set; }
    }    
}

