using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nvs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Nvs.Controllers
{
 	
    public class ghome
    {
        public List<Models.User> Users {get; set; }
        public List<Models.Sensor> Sensors {get; set; }
    }

    public class HomeController : Controller
    {

        private readonly Nvs.Models.Postgresql.DataBaseContext _context;
        private readonly Nvs.Models.SqLite.DataBaseContext _oldcontext;
        private readonly Library.IUserRepository _userRepository;

        public HomeController (Nvs.Models.Postgresql.DataBaseContext context, Nvs.Models.SqLite.DataBaseContext oldcontext, Library.IUserRepository userRepository)
        {
            _context = context;
            _oldcontext = oldcontext;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            Models.User user = default(Models.User);

            await MigrateDb();

            /*
            if (_context.Users.Count() == 0)
            {

                
                //user = _context.Users
                //.Include(q => q.LogItems)
                //.Include(q => q.Sensors)
                //.FirstOrDefault(q => q.Name == "Bas")
                //;
                 
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
            */

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

            /*
            ghome myghome = new ghome();
            myghome.Users = await _context.Users.OrderBy(q => q.Name).ToListAsync();
            myghome.Sensors = await _context.Sensors.Include(s => s.LogItems).OrderBy(q => q.RegisterDate).ToListAsync();

            return View(myghome);
            */

            return View();
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

            ViewBag.User = User.Identity;

            ViewBag.UserII = _userRepository.CurrentUser;

            
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



       public async Task<IActionResult> MigrateDb()
        {

            if (_context.Users.Count() == 0)
            {

                List<Models.User> users = _oldcontext.Users.OrderBy(q => q.UserId).ToList();

                if (users.Count > 0)
                {
                    _context.Users.AddRange(users);
                    await _context.SaveChangesAsync();

                    List<Models.Sensor> sensors = _oldcontext.Sensors.OrderBy(q => q.SensorId).ToList();
                    _context.Sensors.AddRange(sensors);
                    await _context.SaveChangesAsync();

                    List<Models.SensorLogItem> sensorLogItems = _oldcontext.SensorLogItems.OrderBy(q => q.SensorLogItemId).ToList();
                    _context.SensorLogItems.AddRange(sensorLogItems);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    Models.User user;

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


            }

            return Ok("DataBase Migrated!");

        }


        public async Task<IActionResult> Test()
        {
            /*
                server: server121.firstfind.nl
                mail: noreply@nattevoetensensor.nl
                user: nattevoetens02
                pw: Th1sIsAL0ngP@ssW0rd4NoR3ply            

            */
            return Ok(await Library.Methods.SendEmailAsync("bas.bem@xs4all.nl", "Bas", "Test www", "Test vanuit de controller", "", false, "noreply@nattevoetensensor.nl","nvs"));
        }


    }
}
