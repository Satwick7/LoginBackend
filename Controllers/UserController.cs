using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Register.Models;

namespace Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngularApp")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public readonly UserContext _context;
        public UserController(IConfiguration config, UserContext context)
        {
            _context = context;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult Create(User user)
        {
            if(_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null) 
            {
                return Ok("Already Exist");
            }

            user.MemberSince = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Success");
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Pwd).FirstOrDefault();
            if (userAvailable != null) {
                return Ok(new JwtService(_config).GenerateToken(
                    userAvailable.UserID.ToString(),
                    userAvailable.FirstName,
                    userAvailable.LastName,
                    userAvailable.Email,
                    userAvailable.Mobile,
                    userAvailable.Gender
                    )
                );
            }

            return Ok("Failure");
        }
    }
}

/*https://localhost:44360/ */
