
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
        public async Task<IActionResult> Get(int id)
        {  
            var user =await _userRepository.Get(id);
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

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingUser =await _userRepository.Get(id);
            if (existingUser == null)
            {
                return NotFound();
            }

          await  _userRepository.Delete(id);

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
