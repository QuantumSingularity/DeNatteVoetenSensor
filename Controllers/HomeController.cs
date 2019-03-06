using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SensHagen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SensHagen.Controllers
{
 	
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
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


            string remoteIp = "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                remoteIp = Request.Headers["X-Forwarded-For"];
            }

            if (string.IsNullOrWhiteSpace(remoteIp))
            {
                remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();
            }


            user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = new Models.User();
                user.Name = User.Identity.Name;
                user.IpAddress = remoteIp;
            }

            return View(user);
        }

        [Authorize]
        public IActionResult Sensors()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
