using aminsys_api.Model;
using Microsoft.AspNetCore.Mvc;

namespace aminsys_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsernameController : ControllerBase
    {


        private readonly List<string> subjects = new List<string>
        {   "Car",
            "Meal",
            "PC",
            "Brother",
            "Empress",
            "Doctor"
        };

        private readonly List<string> descriptions = new List<string>
        {   "TheReal",
            "Invincible",
            "Mystic",
            "Aloof",
            "Awesome",
            "Majestic",
            "Green"

        };
        Random r = new Random();

        private readonly ILogger<UsernameController> _logger;

        public UsernameController(ILogger<UsernameController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Username GenerateUserName()
        {
            var u1 = descriptions[r.Next(0, descriptions.Count)];
            var u2 = subjects[r.Next(0, subjects.Count)];

            return new Username {
                Id = Guid.NewGuid(),
                UsernameString = u1 + " " + u2 
            };
        }
    }
}
