using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Principal;


/*
 * Change injected object at runtime
 * https://stackoverflow.com/questions/29113206/change-injected-object-at-runtime
 */

namespace Nvs.Library
{

    public enum LogType
    {
        Default = 0,
        Error = 1
    }

    public interface IUserRepository
    {
        User CurrentUser { get; }

        void OnLog(LogType logType, System.Enum logCategory, string method, string parameters, string message);

    }

    public class UserRepository : IUserRepository
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly Nvs.Models.Postgresql.DataBaseContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, Nvs.Models.Postgresql.DataBaseContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public User CurrentUser 
        { 
            get
            {
                return new User(_httpContextAccessor, _configuration, _context, _logger);
            }
        }

        public void OnLog(LogType logType, System.Enum logCategory, string method, string parameters, string message)
        {
            //
        }

    }


}
