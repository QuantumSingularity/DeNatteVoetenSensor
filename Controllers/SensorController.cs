using System;
using System.Net;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SensHagen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace SensHagen.Controllers
{
 	/*
        http://rpi3a.bem.dmz:8443/api/Sensor/Register?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EmailAddress%22%3A%22bas@nattevoetensensor.nl%22}
        http://rpi3a.bem.dmz:8443/api/Sensor/Event?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EventType%22%3A%22heartbeat%222C%22BatteryVoltage%22%3A%223.67%22}

        http://api.nattevoetensensor.nl:16384/api/Sensor/Register?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EmailAddress%22%3A%22bas@nattevoetensensor.nl%22}
        http://api.nattevoetensensor.nl:16384/api/Sensor/Event?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EventType%22%3A%22heartbeat%22%2C%22BatteryVoltage%22%3A3.2367}
        http://api.nattevoetensensor.nl:16384/api/Sensor/Event?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EventType%22%3A%22heartbeat%22}

        http://localhost:8080/api/Sensor/Register?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EmailAddress%22%3A%22bas@nattevoetensensor.nl%22}
        http://localhost:8080/api/Sensor/Event?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EventType%22%3A%22heartbeat%22%2C%22BatteryVoltage%22%3A3.67}
        http://localhost:8080/api/Sensor/Event?data={%22MacAddress%22%3A%22b8:ae:ed:7c:9c:b1%22%2C%22EventType%22%3A%22heartbeat%22}
    */



    [Route("api/[controller]")]
    public class SensorController : Controller
    {


        private readonly SensHagen.Models.DataBaseContext _context;

        public SensorController (SensHagen.Models.DataBaseContext context)
        {
            _context = context;
        }


        [HttpGet, Route("Register")]        
        public async Task<IActionResult> Register(string data)
        {
            // Input: MacAddress, EmailAddress, SensorName
            // Create record
            // Log
            // return ok/nok

            bool isOk = false;
            RegisterData registerData = default(RegisterData);

            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    try
                    {
                        registerData = JsonConvert.DeserializeObject<RegisterData>(data);
                    }
                    catch (Exception exception)
                    {
                        int iex = -1;  //for debug breakpoint
                    }

                    int i = -1;  //for debug breakpoint

                }
            }

            if (registerData != null)
            {

                string result = registerData.AreAllValuesGood();

                if (result == "OK")
                {
                    // all ok? 
                    //   save to database

                    Models.User user = default(Models.User);
                    Models.Sensor sensor = default(Models.Sensor);

                    sensor = _context.Sensors
                        .FirstOrDefault(q => q.MacAddress == registerData.MacAddress)
                    ;

                    user = _context.Users
                        .FirstOrDefault(q => q.EmailAddress == registerData.EmailAddress)
                    ;

                    if (sensor == null && user != null)
                    {
                        sensor = new Sensor();
                        sensor.MacAddress = registerData.MacAddress;
                        sensor.Name = registerData.SensorName;

                        user.Sensors.Add(sensor);

                        await _context.SaveChangesAsync();

                        isOk = true;
                    }
                    else
                    {
                        // Log error
                        // Log Error to database.
                    }
                    
                }
                else
                {
                    // Log error
                    // Log Error to database.
                }

            }


            if (isOk)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet, Route("Event")]        
        public async Task<IActionResult> Event(string data)
        {
            // Input: MacAddress, EventData
            // Create record
            // Log
            // return ok/nok

            bool isOk = false;
            EventData eventData = default(EventData);

            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    try
                    {
                        eventData = JsonConvert.DeserializeObject<EventData>(data);
                    }
                    catch (Exception exception)
                    {
                        int iex = -1;  //for debug breakpoint
                    }

                    int i = -1;  //for debug breakpoint

                }
            }

            if (eventData != null)
            {
                // check input

               string result = eventData.AreAllValuesGood();

                if (result == "OK")
                {
                    // all ok? 
                    //   save to database

                    Models.Sensor sensor = default(Models.Sensor);

                    sensor = _context.Sensors
                        .FirstOrDefault(q => q.MacAddress == eventData.MacAddress)
                    ;

                    if (sensor != null)
                    {

                        SensorLogItem sensorLogItem = new SensorLogItem();
                        sensorLogItem.LogType = eventData.LogItemType;

                        if (eventData?.BatteryVoltage > 0.0)
                        {
                            sensorLogItem.BatteryVoltage = eventData.BatteryVoltage;
                        }

                        sensor.LogItems.Add(sensorLogItem);

                        await _context.SaveChangesAsync();

                        isOk = true;
                    }
                    else
                    {
                        // Log error
                        // Log Error to database.
                    }
                    
                }
                else
                {
                    // Log error
                    // Log Error to database.
                }


            }



            if (isOk)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

    }

    public class RegisterData
    {
        public string MacAddress {get;set;}
        public string EmailAddress {get;set;}
        public string SensorName {get;set;}    // Right now or later on the MySensor page?


        public string AreAllValuesGood()
        {

            // check input

            bool isOk = true;
            string errorMessage = "";

            if (!String.IsNullOrWhiteSpace(MacAddress))
            {
                // Via RegEx:   ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$

                Regex regex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
                Match match = regex.Match(MacAddress);
                if (!match.Success)
                {
                    // Invalid MacAddress
                    isOk = false;
                    errorMessage += $"Invalid MacAddress {MacAddress}";
                }
            } 

            if (!String.IsNullOrWhiteSpace(EmailAddress))
            {

                try
                {
                    System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(EmailAddress);
                }
                catch (FormatException)
                {
                    // Invalid MacAddress
                    isOk = false;
                    errorMessage += $", Invalid EmailAddress {EmailAddress}";
                }
                
            } 

            if (isOk)
            {
                return "OK";
            }
            else
            {
                return $"NOK: {errorMessage}";
            }

        }


    }

    public class EventData
    {
        public string MacAddress {get;set;}
        public string EventType {get;set;}   // HeartBeat, Help, AllGood
        public double? BatteryVoltage {get;set;}

        public Models.SensorLogItemType LogItemType {get; private set;}

        public string AreAllValuesGood()
        {

            // check input

            bool isOk = true;
            string errorMessage = "";

            if (!String.IsNullOrWhiteSpace(MacAddress))
            {
                // Via RegEx:   ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$

                Regex regex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
                Match match = regex.Match(MacAddress);
                if (!match.Success)
                {
                    // Invalid MacAddress
                    isOk = false;
                    errorMessage += $"Invalid MacAddress {MacAddress}";
                }
            } 

            if (!String.IsNullOrWhiteSpace(EventType))
            {

                // Check
                Models.SensorLogItemType sensorLogItemType = Models.SensorLogItemType.Unknown;

                //
                if (Enum.TryParse<Models.SensorLogItemType>(EventType, true, out sensorLogItemType))
                {

                    if (sensorLogItemType != Models.SensorLogItemType.Unknown)
                    {
                        // Looks Good
                        this.LogItemType = sensorLogItemType;
                    }
                    else
                    {
                        isOk = false;
                        errorMessage += $"Invalid EventType {EventType}";
                    }
                }
                
            } 

            if (isOk)
            {
                return "OK";
            }
            else
            {
                return $"NOK: {errorMessage}";
            }

        }



    }




        /*

    public class Example
    {



        public async void  Index()
        {
            Models.User user = default(Models.User);
            Models.Sensor sensor = default(Models.Sensor);

            using (Models.DataBaseContext context = new Models.DataBaseContext())
            {
                user = context.Users
                .Include(q => q.LogItems)
                .Include(q => q.Sensors)
                .FirstOrDefault(q => q.Name == "Bas")
                ;

                if (user == null)
                {
                    user = new Models.User();
                    user.Name = "Bas";
                    user.Email = "bas@nattevoetensensor.nl";
                    context.Users.Add(user);
                    await context.SaveChangesAsync();   // to retrieve the UserId
                    user.SetPassword("none");
                    await context.SaveChangesAsync();

                    sensor = new Models.Sensor();
                    sensor.Name = "Bas01";
                    sensor.Location = "Unknown";
                    await context.SaveChangesAsync();

                    user.Sensors.Add(sensor);
                    await context.SaveChangesAsync();

                }

                Models.UserLogItem userLogItem = new Models.UserLogItem();
                userLogItem.LogItemType = "Index";
                user.LogItems.Add(userLogItem);
                await context.SaveChangesAsync();

                Models.SensorLogItem sensorLogItem = new Models.SensorLogItem();
                sensorLogItem.LogType = SensorLogItemType.Heartbeat;
                user.Sensors[0].LogItems.Add(sensorLogItem);
                await context.SaveChangesAsync();

            }

        }

    }

            */


}
