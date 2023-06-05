using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityTutorial.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController
    {
        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            return new List<string> { "Ahsan", "Pro", "Great" };
        }
    }
}
