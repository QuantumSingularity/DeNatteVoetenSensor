using System;
using System.ComponentModel.DataAnnotations;

namespace SensHagen.Models.LoginViewModels
{

    public class LoginModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityCode { get; set; }
        public string Location { get; set; }


        //[Required(ErrorMessage = "Enter a price"), Range(1, 1000, ErrorMessage = "Min price: 1, max price: 1000")]
        [Display(Name = "Remember Me")]
        public bool RememberMe {get; set; } = true;
    }


}
