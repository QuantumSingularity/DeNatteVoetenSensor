using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SensHagen.Models;
using Microsoft.EntityFrameworkCore;

namespace SensHagen.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Models.User user = default(Models.User);

            using (Models.DataBaseContext context = new Models.DataBaseContext())
            {
                user = context.Users
                .Include(q => q.LogItems)
                .FirstOrDefault(q => q.Name == "Bas")
                ;

                if (user == null)
                {
                    user = new Models.User();
                    user.Name = "Bas";
                    user.Password = "none";
                    user.Email = "bas@nattevoetensensor.nl";
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                Models.UserLogItem logItem = new Models.UserLogItem();
                logItem.LogItemType = "Index";
                user.LogItems.Add(logItem);
                await context.SaveChangesAsync();
            }

            return View(user);
        }

        public IActionResult Privacy()
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
