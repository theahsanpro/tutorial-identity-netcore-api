using IdentityTutorial.Models;
using IdentityTutorial.Models.Authentication.Signup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace IdentityTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authenticationcontroller : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public Authenticationcontroller(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IEmailService emailService)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            // Check if user exists
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if(userExist != null) 
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User Already Exist" });
            }

            // Add the user in the database
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username
            };

            if(await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User Creation Failed" });
                }

                // Add Role to the user
                await _userManager.AddToRoleAsync(user,role);

                // Sending Confirmation Email to the user
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new {token, email = user.Email });
                var message = new Message(new string[] { user.Email! }, "Confirmation email Link", confirmationLink!);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status201Created,
                        new Response { Status = "success", Message = "User Creation Successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This role Does not exist" });
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified successfully" });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = " this user does not exist" });
        }
    }
}
