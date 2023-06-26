using Chat.DominModel.Context;
using Chat.DominModel.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Chat.DominModel.Helper
{
    public class ChatHub : Hub
    {

        private readonly ChatDbContext _context;

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiver, string chat)
        {
            var sender = Context.User.Identity.Name;
            var timestamp = DateTime.UtcNow;

            // Save the message to the database
            var message = new Message
            {
                Sender = sender,
                Receiver = receiver,
                Timestamp = timestamp,
                Content = chat
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Send the message to the receiver
            await Clients.User(receiver).SendAsync("ReceiveMessage",
                new Message
                {
                    Receiver = receiver,
                    Content = chat,
                    Timestamp = timestamp
                });
        }
    }
}

