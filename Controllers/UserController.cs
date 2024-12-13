using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Register.Models;
<<<<<<< HEAD
using ServiceLayer;
=======
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a

namespace Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngularApp")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

<<<<<<< HEAD
        private readonly IUserService _userService;
        
        public UserController(IConfiguration config,IUserService userService)
        {
            _config = config;
            _userService = userService;
=======
        public readonly UserContext _context;
        public UserController(IConfiguration config, UserContext context)
        {
            _context = context;
            _config = config;
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult Create([FromBody] CreateUserRequest request)
        {
            var user = request.User;
            var patient = request.Patient;

<<<<<<< HEAD
            if (_userService.GetUser(user) != null)
=======
            if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null)
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
            {
                return Ok("Already Exist");
            }
            user.MemberSince = DateTime.Now;
<<<<<<< HEAD

            var userValidation = _userService.AddUser(user);

            patient.UserID = user.UserID;

            var patientValidation = _userService.AddPatient(patient);

            if(userValidation && patientValidation)
            {
                return Ok("Success");
            }

            return Ok("Failure");
=======
            _context.Users.Add(user);
            _context.SaveChanges();

            patient.UserID = user.UserID;
            _context.Patients.Add(patient);
            _context.SaveChanges();
            return Ok("Success");
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
        }

        [AllowAnonymous]
        [HttpPost("CreateDoctorUser")]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            var user = request.User;
            var doctor = request.Doctor;

<<<<<<< HEAD
            if (_userService.GetUser(user) != null)
=======
            if (_context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null)
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
            {
                return Ok("Already Exist");
            }

            user.MemberSince = DateTime.Now;
<<<<<<< HEAD

            var userValidation = _userService.AddUser(user);

            doctor.UserID = user.UserID;

            var doctorValidation = _userService.AddDoctor(doctor);

            if (userValidation && doctorValidation)
            {
                return Ok("Success");
            }

            return Ok("Failure");
=======
            _context.Users.Add(user);
            _context.SaveChanges();

            doctor.UserID = user.UserID;
            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            return Ok("Success");
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            //var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Pwd).FirstOrDefault();
            //first taking the Patient & Doctor Data
<<<<<<< HEAD

            List<User> users = _userService.GetAllUsers();
            List<Doctor> doctors = _userService.GetAllDoctors();
            List<Patient> patients = _userService.GetAllPatients();

            var userAvailable = (from u in users
                                 join p in patients on u.UserID equals p.UserID
=======
            var userAvailable = (from u in _context.Users
                                 join p in _context.Patients on u.UserID equals p.UserID
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
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
<<<<<<< HEAD
                                 from u in users
                                 join d in doctors on u.UserID equals d.UserID
=======
                                 from u in _context.Users
                                 join d in _context.Doctors on u.UserID equals d.UserID
>>>>>>> 66fd50469eb94b86450f33b9e0dfb8b2c80e511a
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
