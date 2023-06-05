using System.ComponentModel.DataAnnotations;

namespace IdentityTutorial.Models.Authentication
{
    public class ResetPassword
    {
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
