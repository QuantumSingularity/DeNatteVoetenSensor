using System;
using System.Net;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nvs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nvs.Controllers
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



    [Route("")]
    public class SensorController : Controller
    {


        private readonly Nvs.Models.Postgresql.DataBaseContext _context;
        private readonly ILogger<SensorController> _logger;

        private readonly string _logFile = "";

        public SensorController (Nvs.Models.Postgresql.DataBaseContext context, ILogger<SensorController> logger)
        {
            _context = context;
            _logger = logger;

            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                _logFile = $"{Environment.GetEnvironmentVariable("HOME")}/log/";
            }
            else
            {
                _logFile = $"{Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")}\\log\\";
            }

            _logger.LogInformation($"LogFile: {_logFile}");
        }


#region Register

        [HttpGet, Route("R")]        
        public async Task<IActionResult> RegisterShort(string d)
        {
            return await Register<RegisterDataShort>(d);
        }

        [HttpGet, Route("api/Sensor/Register")]        
        public async Task<IActionResult> RegisterLong(string data)
        {
            return await Register<RegisterData>(data);
        }

        private async Task<IActionResult> Register<T>(string data) where T : RegisterData
        {
            // Input: MacAddress, EmailAddress, SensorName
            // Create record
            // Log
            // return ok/nok

            this.OnLog("Register",data);

            bool isOk = false;
            string errorMessage = "";

            try
            {

                T registerData = RegisterData.GetInstance<T>(data);

                if (registerData.IsValid)
                {

                    // all ok? 
                    //   save to database

                    Models.User user = default(Models.User);
                    Models.Sensor sensor = default(Models.Sensor);

                    sensor = _context.Sensors
                        .Include(q => q.User)
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

                    if (!isOk && sensor != null && user != null)
                    {

                        sensor.Name = registerData.SensorName;
                        sensor.ReRegisterDate = DateTime.Now;

                        // What to do if the user is changed ?
                        // Move sensor to new user.

                        Models.User userOld = sensor.User;
                        if (userOld != null && user.UserId != userOld.UserId)
                        {
                            userOld.Sensors.Remove(sensor);
                            user.Sensors.Add(sensor);
                        }

                        await _context.SaveChangesAsync();

                        isOk = true;
                    }

                    if (!isOk)
                    {

                        if (user == null)
                        {
                            errorMessage = "Invalid User. This EmailAddress is not registered.";
                        }

                        // Log error
                        // Log Error to database.
                    }
                        
                }
                else
                {
                    errorMessage = registerData.ErrorMessage;
                    // Log error
                    // Log Error to database.
                }


                if (isOk)
                {
                    return Ok("Registration Succeeded.");
                }
                else
                {
                    this.OnLog("Register",errorMessage);
                    return StatusCode(400, $"{errorMessage}\n{data}");
                }

            }
            catch (Exception ex)
            {
                this.OnLog("Register",errorMessage, ex);
                return StatusCode(400, $"An Exception has Occurred.\n{data}");
            }

        }

#endregion



#region Event

// {"ID":"60:3C:90:A4:AE:30","E":"Off","V":4.07}
// https://www.nattevoetensensor.nl/E?d={%22ID%22:%2260:3C:90:A4:AE:30%22,%22E%22:%22Off%22,%22V%22:4.07}
// https://www.nattevoetensensor.nl/E?d={%22ID%22:%2260:3C:90:A4:AE:30%22,%22E%22:%22On%22,%22V%22:4.01}
        [HttpGet, Route("E")]        
        public async Task<IActionResult> EventShort(string d)
        {
            return await Event<EventDataShort>(d);
        }

        [HttpGet, Route("api/Sensor/Event")]            
        public async Task<IActionResult> EventLong(string data)
        {
            return await Event<EventData>(data);
        }

        
        private async Task<IActionResult> Event<T>(string data) where T : EventData
        {
            // Input: MacAddress, EventData
            // Create record
            // Log
            // return ok/nok

            this.OnLog("Event",data);

            bool isOk = false;
            string errorMessage = "";


            try
            {

                T eventData = EventData.GetInstance<T>(data);

                if (eventData.IsValid)
                {
                    // check input

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

                        switch (eventData.LogItemType)
                        {
                            case SensorLogItemType.Heartbeat:
                                sensor.HeartBeatDate = DateTime.Now;
                                break;

                            case SensorLogItemType.DetectionOn:
                                sensor.LastDetectionOnDate = DateTime.Now;
                                sensor.DetectionStatusDate = DateTime.Now;
                                sensor.DetectionStatus = "on";
                                break;

                            case SensorLogItemType.DetectionOff:
                                sensor.LastDetectionOffDate = DateTime.Now;
                                sensor.DetectionStatusDate = DateTime.Now;
                                sensor.DetectionStatus = "off";
                                break;

                            default:
                                break;
                        }

                        if (eventData.BatteryVoltage != null && eventData.BatteryVoltage > 0.5)
                        {
                            sensor.BatteryVoltage = eventData.BatteryVoltage;
                            sensor.BatteryVoltageDate = DateTime.Now;
                        }

                        await _context.SaveChangesAsync();

                        isOk = true;
                    }
                    else
                    {
                        errorMessage = "Invalid Sensor. This MacAddress is not registered.";
                        // Log error
                        // Log Error to database.
                    }
                    

                }
                else
                {
                    errorMessage = eventData.ErrorMessage;
                    // Log error
                    // Log Error to database.
                }


                if (isOk)
                {
                    return Ok("Event Processed.");
                }
                else
                {
                    this.OnLog("Event",errorMessage);
                    return StatusCode(400, $"{errorMessage}\n{data}");
                }

            }
            catch (Exception ex)
            {
                this.OnLog("Event",errorMessage, ex);
                return StatusCode(400, $"An Exception has Occurred.\n{data}");
            }


        }

        private void OnLog(string method, string value)
        {
            OnLog(method, value, null);
        }

        private void OnLog(string method, string value, Exception exception)
        {
            try
            {

                string remoteIp = "";
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    remoteIp = Request.Headers["X-Forwarded-For"];
                }
                if (string.IsNullOrWhiteSpace(remoteIp))
                {
                    remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();
                }

                string host = "";
                host = Request.Host.Host;
                if (String.IsNullOrWhiteSpace(host))
                {
                    host = "Unknown";
                }
                if (exception == null)
                {
                    System.IO.File.AppendAllText($"{_logFile}{host}.{DateTime.Now.ToString("yyyy-MM-dd")}.log",$"{DateTime.Now.ToString("yyyy-MM-dd.HH:mm:ss")} [{host}] {method}: [{remoteIp}] {value}\n");
                }
                else
                {
                    string message = $"{exception.Message}\r\n{exception.StackTrace}";
                    if (exception.InnerException != null)
                    {
                        message += $"{exception.InnerException.Message}\r\n{exception.InnerException.StackTrace}";
                    }
                    System.IO.File.AppendAllText($"{_logFile}{host}.{DateTime.Now.ToString("yyyy-MM-dd")}.Exception.log",$"{DateTime.Now.ToString("yyyy-MM-dd.HH:mm:ss")} [{host}] {method}: [{remoteIp}] {value}\n{message}\n");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(default(EventId),ex,method,null);
            }

        }

#endregion



#region API

    [HttpGet, Route("api/GetSensorData")]  
        public async Task<IActionResult> GetSensorData()
        {

            //List<Models.Sensor> sensors;

            Nvs.Controllers.ghome myghome = new ghome();
            myghome.Users = await _context.Users.OrderBy(q => q.Name).ToListAsync();
            //myghome.Sensors = await _context.Sensors.Include(s => s.LogItems).OrderBy(q => q.RegisterDate).ToListAsync();
            myghome.Sensors = await _context.Sensors.OrderBy(q => q.RegisterDate).ToListAsync();

            this.OnLog("API","/api/GetSensorData");

            return PartialView("Partial/Sensors", myghome);

        }


//https://upstream.nattevoetensensor.nl/smartZwolleHub.json
//    [HttpGet, Route("api/GetSensorDataJSON")]  
    [HttpGet, Route("smartZwolleHub.json")]  
        public async Task<IActionResult> GetSensoGetSensorDataJSONData()
        {

            /*
                mijn plan was: LastUpdate = laatste statust update (hoog/laag); lastactive = laatste bericht overall (incl. HB&R).
            */

            List<Models.SensorData> sensorsData = new List<Models.SensorData>();

            List<Models.Sensor> sensors = await _context.Sensors.OrderBy(q => q.RegisterDate).ToListAsync();
            //List<Models.Sensor> sensors = await _context.Sensors.OrderBy(q => q.RegisterDate).ToListAsync();

            foreach (Models.Sensor sensor in sensors)
            {
                Models.SensorData sensorData = new Models.SensorData();
                sensorData.ID = sensor.MacAddress;
                sensorData.Type = "NVS";
                sensorData.Status = "UNKNOWN";
                if (sensor.DetectionStatus == "on") { sensorData.Status = "HIGH";}
                if (sensor.DetectionStatus == "off") { sensorData.Status = "LOW";}
                sensorData.LastUpdate = sensor.LastDetectionOnDate?.ToUniversalTime();
                sensorData.LastActive = sensor.HeartBeatDate?.ToUniversalTime();

                if (sensorData.LastUpdate != null && sensorData.LastActive != null)
                {
                    if (sensorData.LastUpdate > sensorData.LastActive)
                    {
                        sensorData.LastActive = sensorData.LastUpdate;
                    }
                }

                sensorData.Location = new Models.SensorData_Location();

                if (sensor.LocationLatitude != null && sensor.LocationLongitude != null)
                {
                    sensorData.Location.Lat = (Double)sensor.LocationLatitude;
                    sensorData.Location.Lon = (Double)sensor.LocationLongitude;
                }

                /*
                List<Models.SensorLogItem> logItems = _context.SensorLogItems.Where(q => q.SensorId == sensor.SensorId).OrderByDescending(q => q.TimeStamp).Take(10).ToList();

                List<Models.SensorLogItem> logItemsOn = sensor.LogItems.Where(q => q.LogType == SensorLogItemType.DetectionOn).OrderByDescending(q => q.TimeStamp).Take(3).ToList();

                sensorData.History =  new List<Models.SensorData_History>();
                foreach (Models.SensorLogItem logItem in logItemsOn)
                {
                    Models.SensorData_History history = new Models.SensorData_History();
                    history.eventID = logItem.SensorLogItemId;
                    history.start = logItem.TimeStamp.ToUniversalTime();

                    try
                    {
                        List<Models.SensorLogItem> logItemsOff = logItems.Where(q => q.LogType == SensorLogItemType.DetectionOff && q.TimeStamp >= logItem.TimeStamp).OrderBy(q => q.TimeStamp).Take(1).ToList();
                        if (logItemsOff.Count > 0)
                        {
                        history.end = logItemsOff[0].TimeStamp.ToUniversalTime();
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "GetSensoGetSensorDataJSONData", null);
                    }

                    sensorData.History.Add(history);
                }


                if (sensorData.History.Count > 0)
                {
                    sensorData.eventID = sensorData.History[0].eventID;
                    sensorData.start = sensorData.History[0].start;
                    sensorData.end = sensorData.History[0].end;
                }
                */

                //List<Models.SensorLogItem> logItems = _context.SensorLogItems.Where(q => q.SensorId == sensor.SensorId).OrderByDescending(q => q.TimeStamp).Take(10).ToList();

                //List<Models.SensorLogItem> logItemsOn = _context.SensorLogItems.Where(q => q.SensorId == sensor.SensorId && q.LogType == SensorLogItemType.DetectionOn).OrderByDescending(q => q.TimeStamp).Take(1).ToList();
                //List<Models.SensorLogItem> logItemsOff = _context.SensorLogItems.Where(q => q.SensorId == sensor.SensorId && q.LogType == SensorLogItemType.DetectionOff).OrderByDescending(q => q.TimeStamp).Take(1).ToList();

                sensorData.start = sensorData.LastUpdate;

                sensorsData.Add(sensorData);

            }

            this.OnLog("API","smartZwolleHub.json");

            return new OkObjectResult(sensorsData);

        }




#endregion


    }



#region RegisterData and EventData

    public class RegisterDataShort : RegisterData
    {

        public RegisterDataShort() : base()
        {

        }        

        [JsonProperty("ID")]
        public override string MacAddress {get;set;}
        [JsonProperty("E")]
        public override string EmailAddress {get;set;}
        [JsonProperty("SN")]
        public override string SensorName {get;set;}  

    }

    public class RegisterData
    {

        public RegisterData()
        {

        }        
        public virtual string MacAddress {get;set;}
        public virtual string EmailAddress {get;set;}
        public virtual string SensorName {get;set;}    // Right now or later on the MySensor page?


        public string ErrorMessage {get; private set;} = "";
        public bool IsValid  {get; private set;} = false;

        public static T GetInstance<T>(string data) where T : RegisterData
        {

            T registerData = Activator.CreateInstance<T>();

            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    try
                    {
                        registerData = JsonConvert.DeserializeObject<T>(data);

                        if (registerData.CheckValues())
                        {
                            // OK
                            registerData.IsValid = true;
                        }
                        else
                        {
                            // NOK
                        }
                    }
                    catch (Exception exception)
                    {
                        int iex = -1;  //for debug breakpoint
                        registerData.ErrorMessage = exception.Message;
                    }

                    int i = -1;  //for debug breakpoint

                }
                else
                {
                    registerData.ErrorMessage = "No valid json received.";
                }        
            }    
            else
            {
                registerData.ErrorMessage = "No json received.";
            }        

            return registerData;
        }



        private bool CheckValues()
        {

            // check input

            bool isOk = true;

            if (!String.IsNullOrWhiteSpace(MacAddress))
            {
                // Via RegEx:   ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$

                MacAddress = MacAddress.ToUpper();

                Regex regex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
                Match match = regex.Match(MacAddress);
                if (!match.Success)
                {
                    // Invalid MacAddress
                    isOk = false;
                    this.ErrorMessage += $"Invalid MacAddress {MacAddress}";
                }
            } 
            else
            {
                // Invalid MacAddress
                isOk = false;
                this.ErrorMessage += $"Invalid MacAddress {MacAddress}";
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
                    this.ErrorMessage += $", Invalid EmailAddress {EmailAddress}";
                }
                
            } 
            else
            {
                // Invalid MacAddress
                isOk = false;
                this.ErrorMessage += $", Invalid EmailAddress {EmailAddress}";
            }

            return isOk;

        }


    }


    public class EventDataShort : EventData
    {

        public EventDataShort() : base()
        {

        }        

        [JsonProperty("ID")]
        public override string MacAddress {get;set;}
        [JsonProperty("E")]
        public override string EventType {get;set;}
        [JsonProperty("V")]
        public override double? BatteryVoltage {get;set;}

    }



    public class EventData
    {
        public EventData()
        {

        }

        public virtual string MacAddress {get;set;}
        public virtual string EventType {get;set;}   // HeartBeat, Help, AllGood
        public virtual double? BatteryVoltage {get;set;}

        public Models.SensorLogItemType LogItemType {get; private set;}


        public string ErrorMessage {get; private set;} = "";
        public bool IsValid  {get; private set;} = false;

        public static T GetInstance<T>(string data) where T : EventData
        {

            T eventData = Activator.CreateInstance<T>();

            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    try
                    {
                        eventData = JsonConvert.DeserializeObject<T>(data);

                        if (eventData.CheckValues())
                        {
                            // OK
                            eventData.IsValid = true;
                        }
                        else
                        {
                            // NOK
                        }
                    }
                    catch (Exception exception)
                    {
                        int iex = -1;  //for debug breakpoint
                        eventData.ErrorMessage = exception.Message;
                    }

                    int i = -1;  //for debug breakpoint

                }
                else
                {
                    eventData.ErrorMessage = "No valid json received.";
                }        
            }    
            else
            {
                eventData.ErrorMessage = "No json received.";
            }        

            return eventData;
        }

        private bool CheckValues()
        {

            // check input

            bool isOk = true;

            if (!String.IsNullOrWhiteSpace(MacAddress))
            {

                MacAddress = MacAddress.ToUpper();

                // Via RegEx:   ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$

                Regex regex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
                Match match = regex.Match(MacAddress);
                if (!match.Success)
                {
                    // Invalid MacAddress
                    isOk = false;
                    this.ErrorMessage += $"Invalid MacAddress {MacAddress}";
                }
            } 
            else
            {
                // Invalid MacAddress
                isOk = false;
                this.ErrorMessage += $"Invalid MacAddress {MacAddress}";
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

                        if ((int)sensorLogItemType > 10)
                        {
                            sensorLogItemType = (Models.SensorLogItemType)Enum.ToObject(typeof(Models.SensorLogItemType), (int)sensorLogItemType - 10);
                        }

                        // Looks Good
                        this.LogItemType = sensorLogItemType;
                    }
                }

                if (sensorLogItemType == SensorLogItemType.Unknown)
                {
                    isOk = false;
                    this.ErrorMessage += $"Invalid EventType {EventType}";
                }

            } 
            else
            {
                isOk = false;
                this.ErrorMessage += $"Invalid EventType";
            }

            return isOk;

        }

    }
#endregion

}
