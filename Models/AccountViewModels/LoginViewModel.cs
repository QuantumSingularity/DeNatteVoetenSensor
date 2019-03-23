using System;
using System.ComponentModel.DataAnnotations;

namespace SensHagen.Models.LoginViewModels
{

    public class LoginModel
    {
        
        [Required(ErrorMessage = "Ongeldig Emailadres")]
        [Display(Name = "Emailadres")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Display(Name = "Wachtwoord")]
        [StringLength(128)]
        [MinLength(8)]
        [Required(ErrorMessage = "Ongeldig Wachtwoord. Minimale lengte is 8 karakters.")]
        public string Password { get; set; }



        [Display(Name = "PIN Code")]
        [StringLength(6)]
        public string SecurityCode { get; set; }

        //[Required(ErrorMessage = "Enter a price"), Range(1, 1000, ErrorMessage = "Min price: 1, max price: 1000")]
        [Display(Name = "Onthoud mijn inloggegevens")]
        public bool RememberMe {get; set; } = true;
    }


}
