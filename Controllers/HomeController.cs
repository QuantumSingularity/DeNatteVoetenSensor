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
 	
    public class ghome
    {
        public List<Models.User> Users {get; set; }
        public List<Models.Sensor> Sensors {get; set; }
    }

    public class HomeController : Controller
    {

        private readonly SensHagen.Models.DataBaseContext _context;

        public HomeController (SensHagen.Models.DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            Models.User user = default(Models.User);


            if (_context.Users.Count() == 0)
            {

                /*
                user = _context.Users
                .Include(q => q.LogItems)
                .Include(q => q.Sensors)
                .FirstOrDefault(q => q.Name == "Bas")
                ;
                 */

                user = new Models.User()
                {
                    Name = "Bas",
                    EmailAddress = "bas@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                user = new Models.User()
                {
                    Name = "Anton",
                    EmailAddress = "anton@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                user = new Models.User()
                {
                    Name = "Sander",
                    EmailAddress = "sander@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                user = new Models.User()
                {
                    Name = "Jan",
                    EmailAddress = "jan@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                user = new Models.User()
                {
                    Name = "Ben",
                    EmailAddress = "ben@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                user = new Models.User()
                {
                    Name = "Ryan",
                    EmailAddress = "ryan@nattevoetensensor.nl"
                };
                user.SetPassword("none");
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

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

            /*
                Models.UserLogItem userLogItem = new Models.UserLogItem();
                userLogItem.LogItemType = "Index";
                user.LogItems.Add(userLogItem);
                await _context.SaveChangesAsync();
            */

            user = new Models.User();
            user.IpAddress = remoteIp;
            if (User.Identity.IsAuthenticated)
            {
                user.Name = User.Identity.Name;
            }


            ghome myghome = new ghome();
            myghome.Users = await _context.Users.OrderBy(q => q.Name).ToListAsync();
            myghome.Sensors = await _context.Sensors.Include(s => s.LogItems).OrderBy(q => q.RegisterDate).ToListAsync();

            return View(myghome);
        }

        public IActionResult Information()
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

            ViewBag.RemoteIp = remoteIp;
            
            return View();
        }

        [Authorize]
        public IActionResult MySensors()
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
