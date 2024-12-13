using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Register.Models;
using ServiceLayer;

namespace Register.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngularApp")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        private readonly IUserService _userService;
        
        public UserController(IConfiguration config,IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public IActionResult Create([FromBody] CreateUserRequest request)
        {
            var user = request.User;
            var patient = request.Patient;

            if (_userService.GetUser(user) != null)
            {
                return Ok("Already Exist");
            }
            user.MemberSince = DateTime.Now;

            var userValidation = _userService.AddUser(user);

            patient.UserID = user.UserID;

            var patientValidation = _userService.AddPatient(patient);

            if(userValidation && patientValidation)
            {
                return Ok("Success");
            }

            return Ok("Failure");
        }

        [AllowAnonymous]
        [HttpPost("CreateDoctorUser")]
        public IActionResult Create([FromBody] CreateDoctorRequest request)
        {
            var user = request.User;
            var doctor = request.Doctor;

            if (_userService.GetUser(user) != null)
            {
                return Ok("Already Exist");
            }

            user.MemberSince = DateTime.Now;

            var userValidation = _userService.AddUser(user);

            doctor.UserID = user.UserID;

            var doctorValidation = _userService.AddDoctor(doctor);

            if (userValidation && doctorValidation)
            {
                return Ok("Success");
            }

            return Ok("Failure");
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public IActionResult Login(Login user)
        {
            //var userAvailable = _context.Users.Where(u => u.Email == user.Email && u.Pwd == user.Pwd).FirstOrDefault();
            //first taking the Patient & Doctor Data

            List<User> users = _userService.GetAllUsers();
            List<Doctor> doctors = _userService.GetAllDoctors();
            List<Patient> patients = _userService.GetAllPatients();

            var userAvailable = (from u in users
                                 join p in patients on u.UserID equals p.UserID
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
                                 from u in users
                                 join d in doctors on u.UserID equals d.UserID
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
