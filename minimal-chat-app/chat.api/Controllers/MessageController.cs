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
        private readonly IMessageRepository _messageRepository;

        public MessageController(IHubContext<ChatHub> chatHub, ChatDbContext context, IUserRepository userRepository,IMessageRepository messageRepository)
        {
            _chatHub = chatHub;
            _context = context;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [Authorize]
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(string receiver, string message)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var sender = User.Identity.Name;
            var timestamp = DateTime.UtcNow;

            // Validation logic for receiver and message
            if (string.IsNullOrEmpty(receiver) || string.IsNullOrEmpty(message))
            {
                return BadRequest();
            }

            try
            {
                // Save the message to the database
                var chatMessage = new Message
                {
                    Sender = sender,
                    Receiver = receiver,
                    Timestamp = timestamp,
                    Content = message
                };

                var createdMessage = await _messageRepository.CreateMessage(chatMessage);

                // Send the message to the receiver
                await _chatHub.Clients.All.SendAsync("ReceiveMessage", createdMessage);

                return Ok(createdMessage);
            }
            catch (Exception)
            {
                return BadRequest();
            }
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
            var message = await _messageRepository.GetMessageById(messageIdGuid);
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
            var isUpdated = await _messageRepository.UpdateMessage(message);

            if (isUpdated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
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
            var message = await _messageRepository.GetMessageById(messageIdGuid);
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
            var isDeleted = await _messageRepository.DeleteMessage(messageIdGuid);

            if (isDeleted)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetConversation(string userId, DateTime? before = null, int count = 20, string sort = "asc")
        {
            try
            {
                var currentUser = User.Identity.Name?.Trim();

                // Check if the current user has access to retrieve the conversation
                if (!string.Equals(currentUser, userId, StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized();
                }

                // Get the conversation messages from the repository
                var messages = await _messageRepository.GetConversationMessages(currentUser, userId, before, count, sort);

                // Prepare the response body
                var response = new
                {
                    messages = messages.Select(m => new
                    {
                        id = m.Id.ToString(),
                        senderId = m.Sender,
                        receiverId = m.Receiver,
                        content = m.Content,
                        timestamp = m.Timestamp
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
                     
        }
    }

}





