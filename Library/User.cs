using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Nvs.Library
{

    public class User 
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly Nvs.Models.Postgresql.DataBaseContext _context;
        private readonly ILogger<UserRepository> _logger;

        public User(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, Nvs.Models.Postgresql.DataBaseContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            UserId = _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        public User(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, Nvs.Models.Postgresql.DataBaseContext context, ILogger<UserRepository> logger, Nvs.Models.User user)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            this.UserName = user.Name;
            this.UserId = user.EmailAddress;
            this.IsDisabled = user.IsDisabled;

        }

        public string UserName { get; set; }
        public string UserId { get; private set; }

        public DateTime LoggedOnDate { get; set; } = DateTime.Now;
        public DateTime LastAccessedDate { get; set; } = DateTime.Now;
        public int NumberOfRequests { get; set; } = 0;

        public bool IsDisabled { get; set; } = false;

    }

}
