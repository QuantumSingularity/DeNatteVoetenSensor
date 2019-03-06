using System;
using System.Net;

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
 	
    public class SensorController : Controller
    {
        
        public SensorController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Register(string data)
        {
            // Input: apiKey, ipAdress
            // Create record
            // Log
            // return id + Key

            //string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            var headers = Request.Headers;

            string result = "";
            foreach (var header in headers)
            {
                result += $"{header.Key} == {header.Value}\n";
            }

            string remoteIp = "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                remoteIp = Request.Headers["X-Forwarded-For"];
            }

            if (string.IsNullOrWhiteSpace(remoteIp))
            {
                remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            result += $"RemoteIp == {remoteIp}\n";


            return Ok(result);


            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    //string jsonData = WebUtility.UrlDecode(data);

                    RegisterData registerData = JsonConvert.DeserializeObject<RegisterData>(data);

                    int i = -1;
                }
            }


            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Event(string data)
        {
            // Input: apiKey, id, eventData
            // Create record
            // Log
            // return status

            if (!String.IsNullOrWhiteSpace(data))
            {
                data = data.Trim();
                if ( data.Length > 10)
                {
                    //string jsonData = WebUtility.UrlDecode(data);

                    EventData registerData = JsonConvert.DeserializeObject<EventData>(data);

                    int i = -1;
                }
            }

            return Ok();
        }

    }

    public class RegisterData
    {
        public string apiKey {get;set;}
    }

    public class EventData
    {
        public string apiKey {get;set;}
        public string sensorId {get;set;}
        public string eventType {get;set;}
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
