using Chat.DominModel.DTOs;
using Chat.DominModel.Model;
using Chat.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Minimal_chat_application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            var response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email
            };

            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userRepository.GetAll();
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
        public IActionResult Insert([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new ErrorResponse
                {
                    Error = "Invalid user data."
                };
                return BadRequest(errorResponse);
            }

            var existingUser = _userRepository.GetByEmail(user.Email);
            if (existingUser != null)
            {
                var errorResponse = new ErrorResponse
                {
                    Error = "Email is already registered."
                };
                return Conflict(errorResponse);
            }

            _userRepository.Insert(user);

            var response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Message = "Registration successful"
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new ErrorResponse
                {
                    Error = "Invalid user data."
                };
                return BadRequest(errorResponse);
            }

            var existingUser = _userRepository.Get(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            user.UserId = id;
            _userRepository.Update(user);

            var response = new UserResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingUser = _userRepository.Get(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _userRepository.Delete(id);

            var response = new UserResponse
            {
                UserId = existingUser.UserId,
                Name = existingUser.Name,
                Email = existingUser.Email
            };

            return Ok(response);
        }
    }

   

   


}
