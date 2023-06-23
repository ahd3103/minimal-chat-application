
using Chat.DominModel.DTOs;
using Chat.DominModel.Model;
using Chat.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Minimal_chat_application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;


        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration; 
        }

        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> GetAll()
        {
            var users =await _userRepository.GetAll();
            var response = users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email
            });

            return Ok(response);
        }

        [HttpPost]
        [Route("/api/register")]
        public async Task<IActionResult> Insert([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new ErrorResponse
                {
                    Error = "Invalid user data."
                };
                return BadRequest(errorResponse);
            }

            var existingUser =await  _userRepository.GetByEmail(user.Email);
            if (existingUser != null)
            {
                var errorResponse = new ErrorResponse
                {
                    Error = "Email is already registered."
                };
                return Conflict(errorResponse);
            }

            await _userRepository.Insert(user);

            var response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Message = "Registration successful"
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userRepository.CheckUser(email, password);

            if (user == null)
            {
                return Unauthorized(); // Login failed due to incorrect credentials
            }

            var token = GenerateJwtToken(user.UserId);

            var userProfile = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                //Password = user.Password
            };

            return Ok(new { Token = token, Profile = userProfile, Massage = "Login successful" });
        }
        // Helper method to generate a JWT token
        private string GenerateJwtToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _configuration["JWT:Secret"];
            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Set the token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

    }

   

   


}
