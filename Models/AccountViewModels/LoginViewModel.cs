using System;
using System.ComponentModel.DataAnnotations;

namespace Nvs.Models.LoginViewModels
{

    public class LoginModel
    {
        
        [Required(ErrorMessage = "Ongeldig Emailadres")]
        [Display(Name = "Emailadres")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Ongeldig Emailadres")]
        public string Username { get; set; }


        [Display(Name = "Wachtwoord")]
        [StringLength(128)]
        [MinLength(8)]
        [Required(ErrorMessage = "Ongeldig Wachtwoord. Minimale lengte is 8 karakters.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Ongeldige Naam")]
        [Display(Name = "Naam")]
        public string Name { get; set; }


        [Display(Name = "PIN Code")]
        [Range(typeof(int), "1000", "9999", ErrorMessage = "De PIN bestaat uit 4 cijfers tussen de {1} en {2}.")]
        [Required(ErrorMessage = "Ongeldige PIN Code. De PIN bestaat uit 4 cijfers.")]
        public int SecurityCode { get; set; }


        //[Required(ErrorMessage = "Enter a price"), Range(1, 1000, ErrorMessage = "Min price: 1, max price: 1000")]
        [Display(Name = "Onthoud mijn inloggegevens")]
        public bool RememberMe {get; set; } = true;


        public string CreateResult {get; set;}

    }


}
