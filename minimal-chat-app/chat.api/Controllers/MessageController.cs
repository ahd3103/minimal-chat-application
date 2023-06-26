using Chat.DominModel.Context;
using Chat.DominModel.Helper;
using Chat.DominModel.Model;
using Chat.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Minimal_chat_application.Controllers
{

    [Route("api/[controller]")]
        [ApiController]
        public class MessageController : ControllerBase
        {

            private readonly IHubContext<ChatHub> _chatHub;
            private readonly ChatDbContext _context;
            private readonly IUserRepository _userRepository;

            public MessageController(IHubContext<ChatHub> chatHub, ChatDbContext context, IUserRepository userRepository)
            {
                _chatHub = chatHub;
                _context = context;
                _userRepository = userRepository;
            }
            [Authorize]
            [HttpPost("sendMessage")]
            public async Task<IActionResult> SendMessage(string reciver, string message)
            {
                var sender = User.Identity.Name;

                var timestamp = DateTime.UtcNow;

                //var username = await _userRepository.Get(request.UserId);

                // Save the message to the database
                var chatMessage = new Message
                {
                    Sender = sender,
                    Receiver = reciver,
                    Timestamp = timestamp,
                    Content = message
                };
                _context.Messages.Add(chatMessage);
                await _context.SaveChangesAsync();

                // Send the message to the receiver
                await _chatHub.Clients.All.SendAsync("ReceiveMessage", new Message
                {
                    Sender = sender,
                    Receiver = reciver,
                    Timestamp = timestamp,
                    Content = message
                });

                return Ok(chatMessage);
            }

            [HttpPut("messages/{messageId}")]
            public async Task<IActionResult> EditMessage(string messageId, [FromBody] string content)
            {
                var sender = User.Identity.Name;

                // Convert the messageId string to Guid
                if (!Guid.TryParse(messageId, out Guid messageIdGuid))
                {
                    return BadRequest(new { error = "Invalid messageId" });
                }

                // Get the message from the database
                var message = await _context.Messages.FindAsync(messageIdGuid);
                if (message == null)
                {
                    return NotFound(new { error = "Message not found" });
                }

                // Check if the message sender is the current user
                if (message.Sender != sender)
                {
                    return Unauthorized();
                }

                // Update the message content
                message.Content = content;
                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                return Ok();
            }

            [HttpDelete("messages/{messageId}")]
            public async Task<IActionResult> DeleteMessage(string messageId)
            {
                var sender = User.Identity.Name;

                // Convert the messageId string to Guid
                if (!Guid.TryParse(messageId, out Guid messageIdGuid))
                {
                    return BadRequest(new { error = "Invalid messageId" });
                }

                // Get the message from the database
                var message = await _context.Messages.FindAsync(messageIdGuid);
                if (message == null)
                {
                    return NotFound(new { error = "Message not found" });
                }

                // Check if the message sender is the current user
                if (message.Sender != sender)
                {
                    return Unauthorized();
                }

                // Delete the message
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                return Ok();
                }
                [HttpGet("conversations/{userId}")]
            public async Task<IActionResult> GetConversation(string userId, DateTime? before = null, int count = 20, string sort = "asc")
            {
            var currentUser = User.Identity.Name;

            // Check if the current user has access to retrieve the conversation
            if (currentUser != userId)
            {
                return Unauthorized();
            }

            // Get the user from the repository or database
            var user = await _userRepository.GetAll(); 

            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Get the conversation messages based on the provided parameters
            var query = _context.Messages.Where(m => (m.Sender == currentUser && m.Receiver == userId) || (m.Sender == userId && m.Receiver == currentUser));

            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp < before);
            }

             if (sort.ToLower() == "desc")
             {
                query = query.OrderByDescending(m => m.Timestamp);
             }
             else
             {
                query = query.OrderBy(m => m.Timestamp);
             }

             var messages = await query.Take(count).ToListAsync();

              // Prepare the response body
              var response = new
             {
                messages = messages.Select(m => new
                {
                    id = m.Id,
                    senderId = m.Sender,
                    receiverId = m.Receiver,
                    content = m.Content,
                    timestamp = m.Timestamp
                })
              };

              return Ok(response);
            }



        }
}



