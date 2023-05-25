using System.ComponentModel.DataAnnotations;

namespace IdentityTutorial.Models.Authentication.Signup
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }
    }
}
