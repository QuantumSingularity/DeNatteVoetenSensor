using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvs.Models
{

    public enum SensorLogItemType
    {
        Unknown = 0,

        Heartbeat = 1,
        DetectionOn = 2,
        DetectionOff = 3,

        HB = 11,
        On = 12,
        Off = 13        

    }


    public class Sensor
    {
        public Sensor()
        {
            LogItems = new List<SensorLogItem>();
            RegisterDate = DateTime.Now;
            DetectionStatus = "-";
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SensorId { get; set; }

        // Unique / AlternateKey
        public string MacAddress { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }

        public DateTime RegisterDate {get; set; }
        public DateTime? ReRegisterDate {get; set; }

        public User User {get; set;}

        public List<SensorLogItem> LogItems { get; set; }

        public DateTime? HeartBeatDate {get; set; }
        public DateTime? LastDetectionOnDate {get; set; }
        public DateTime? LastDetectionOffDate {get; set; }

        public string DetectionStatus {get; set;}
        public DateTime? DetectionStatusDate {get; set; }

        public DateTime? BatteryVoltageDate {get; set; }
        public double? BatteryVoltage { get; set; }

        public Double? LocationLatitude { get; set; }
        public Double? LocationLongitude { get; set; }

    }


    public class SensorLogItem
    {

        public SensorLogItem()
        {
            this.TimeStamp = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserLogItemId { get; set; }
        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }
        public string LogItemType { get; set; }
        public string Value { get; set; }

        public User User { get; set; }
    }    


    #region Anton API
    public class SensorData_Location
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class SensorData_History
    {
        public int eventID { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
    }

    public class SensorData
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? LastActive { get; set; }
        public SensorData_Location Location { get; set; }
        public List<SensorData_History> History { get; set; }

        public int eventID { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
                
    }

    #endregion


}

