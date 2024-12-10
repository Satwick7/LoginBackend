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
        public IActionResult Create([FromBody] CreateUserRequest request)
        {
            var user = request.User;
            var patient = request.Patient;

            if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null)
            {
                return Ok("Already Exist");
            }
            user.MemberSince = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();

            patient.UserID = user.UserID;
            _context.Patients.Add(patient);
            _context.SaveChanges();
            return Ok("Success");
        }

        [AllowAnonymous]
        [HttpPost("CreateDoctorUser")]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            var user = request.User;
            var doctor = request.Doctor;

            if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null)
            {
                return Ok("Already Exist");
            }

            user.MemberSince = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();

            doctor.UserID = user.UserID;
            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            return Ok("Success");
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            //var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Pwd).FirstOrDefault();
            //first taking the Patient & Doctor Data
            var userAvailable = (from u in _context.Users
                                 join p in _context.Patients on u.UserID equals p.UserID
                                 where u.Email == user.Email && u.Pwd == user.Pwd
                                 select new
                                 {
                                     u.UserID,
                                     u.FirstName,
                                     u.LastName,
                                     u.Email,
                                     u.Mobile,
                                     u.Gender,
                                     TypeOfUser = "Patient"
                                 })
                                .Union(
                                 from u in _context.Users
                                 join d in _context.Doctors on u.UserID equals d.UserID
                                 where u.Email == user.Email && u.Pwd == user.Pwd
                                 select new
                                 {
                                     u.UserID,
                                     u.FirstName,
                                     u.LastName,
                                     u.Email,
                                     u.Mobile,
                                     u.Gender,
                                     TypeOfUser = "Doctor"
                                 })
                                .FirstOrDefault();


            if (userAvailable != null)
            {
                var token = new JwtService(_config).GenerateToken(
                    userAvailable.UserID.ToString(),
                    userAvailable.FirstName,
                    userAvailable.LastName,
                    userAvailable.Email,
                    userAvailable.Mobile,
                    userAvailable.Gender
                    );

                return Ok(new
                {
                    Token = token,
                    User = userAvailable
                }
                );
            }

            return Ok("Failure");
        }
    }
}

/*https://localhost:44360/ */
