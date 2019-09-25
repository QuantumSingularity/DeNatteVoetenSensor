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

    public enum LogCategory
    {
        Default = 0,
        Register = 1,
        LogOn = 2,

    }

    public interface IUserRepository
    {
        User CurrentUser { get; }

        void OnLog(LogType logType, LogCategory logCategory, string method, string parameters, string message);
        Task<string> CreateUser(string username, string password, int securityCode, string name);
        Task<string> LogOnUser(string username, string password);

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

                string emailAddress = _httpContextAccessor.HttpContext.User.Identity.Name;

                if (String.IsNullOrWhiteSpace(emailAddress))
                {
                    return new User(_httpContextAccessor, _configuration, _context, _logger);
                }
                else
                {
                    Models.User user = _context.Users.FirstOrDefault( q => q.EmailAddress == emailAddress );
                    if (user != null)
                    {
                        return new User(_httpContextAccessor, _configuration, _context, _logger, user);
                    }
                    else
                    {
                        return new User(_httpContextAccessor, _configuration, _context, _logger);
                    }
                }

            }
        }

        public async Task<string> CreateUser(string username, string password, int securityCode, string name)
        {
            string result = "OK";

            if (securityCode != 1234)
            {
                result = "Ongeldige PIN Code.";
            }

            Models.User user = await _context.Users.FirstOrDefaultAsync( q => q.EmailAddress == username );

            if (user != null)
            {
                result = "De gebruikersnaam bestaat al. Als je je wachtwoord bent vergeten kun je `wachtwoord vergeten` aanklikken op de `Inloggen pagina`.";
                OnLog(LogType.Error, LogCategory.Register, "CreateUser","","Failure. User already exists.", user);
            }
            else
            {
                user = new Models.User();
                user.EmailAddress = username;
                user.SetPassword(password);
                user.Name = name;
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                OnLog(LogType.Default, LogCategory.Register, "CreateUser","","Success.", user);
            }

            return result;
        }


        public async Task<string> LogOnUser(string username, string password)
        {
            string result = "Ongeldige combinatie van Emailadres/Wachtwoord!";

            Models.User user = await _context.Users.FirstOrDefaultAsync( q => q.EmailAddress == username );

            if (user != null)
            {
                if (user.IsPasswordCorrect(password))
                {
                    user.LastLogOnDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    OnLog(LogType.Default, LogCategory.LogOn, "LogOnUser","","Success.", user);
                    result = "OK";
                }
                else
                {
                    OnLog(LogType.Error, LogCategory.LogOn, "LogOnUser","","Failure. Invalid Password.", user);
                }

            }

            return result;
        }

        public void OnLog(LogType logType, LogCategory logCategory, string method, string parameters, string message)
        {
            string emailAddress = _httpContextAccessor.HttpContext.User.Identity.Name;

            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                Models.User user = _context.Users.FirstOrDefault( q => q.EmailAddress == emailAddress );
                if (user != null)
                {
                    OnLog(logType, logCategory, method, parameters, message, user);
                }
            }
        }

        public void OnLog(LogType logType, LogCategory logCategory, string method, string parameters, string message, Models.User user)
        {
            Models.UserLogItem userLogItem = new Models.UserLogItem();

            userLogItem.LogType = logType.ToString();
            userLogItem.LogCategory = logCategory.ToString();
            userLogItem.Method = method;
            userLogItem.Parameters = parameters;
            userLogItem.Message = message;
            user.LogItems.Add(userLogItem);
            _context.SaveChanges(); 

        }

    }


}
