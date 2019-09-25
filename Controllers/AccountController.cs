using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


namespace Nvs.Controllers
{

    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {


        private readonly ILogger _logger;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        private readonly Library.IUserRepository _userRepository;

        public AccountController(ILogger<AccountController> logger, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor, Library.IUserRepository userRepository)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    
        [HttpPost]
        public async Task<IActionResult> Login(Nvs.Models.LoginViewModels.LoginModel loginModel)
        {
            
            if (ModelState.IsValid)
            {

                string result = await _userRepository.LogOnUser(loginModel.Username.ToLower().Trim(), loginModel.Password);

                if (result.StartsWith("OK"))
                {


    /*     
                    var userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(principal);
    */              
                    

                    #region snippet1
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginModel.Username),
                        new Claim(ClaimTypes.WindowsAccountName, Guid.NewGuid().ToString()),
                        //new Claim("FullName", user.FullName),
                        new Claim(ClaimTypes.Role, "Administrator"),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {

                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(125)
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. Required when setting the 
                        // ExpireTimeSpan option of CookieAuthenticationOptions 
                        // set with AddCookie. Also required when setting 
                        // ExpiresUtc.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    if (loginModel.RememberMe)
                    {
                        authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);
                        authProperties.IsPersistent = true;
                    }

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), 
                        authProperties);
                    #endregion


                    //Just redirect to our index after logging in. 
                    return Redirect("/");
                }
                else
                {
                    loginModel.CreateResult = result;
                }
            }
            else
            {
                loginModel.CreateResult = "Ongeldige Invoer.";
            }

            return View(loginModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"User {User.Identity.Name} logged out at {DateTime.UtcNow}.");
            
            #region snippet1
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            return Redirect("/");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Manage()
        {

            if (User.Identity.IsAuthenticated)
            {
                Models.LoginViewModels.LoginModel loginModel = new Models.LoginViewModels.LoginModel();
                loginModel.Username = User.Identity.Name;

                return View(loginModel);

            }
            else
            {
                return Redirect("/");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Nvs.Models.LoginViewModels.LoginModel loginModel)
        {

            loginModel.CreateResult = await _userRepository.CreateUser(loginModel.Username.ToLower().Trim(), loginModel.Password, loginModel.SecurityCode, loginModel.Name);
            
            if (loginModel.CreateResult.StartsWith("OK"))
            {
                return Redirect("/Account/Login");
            }
            else
            {
                return View(loginModel);
            }
        }


/*

        // GetIp when someone had a failed login.
        // When there are many failed logins from 1 IP, put the IP into the IpTables BlackList.

            #region GetIp

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

            #endregion


 */

    }

}