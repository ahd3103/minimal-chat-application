using Chat.DominModel.Context;
using Chat.DominModel.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public class MessageRepository : IMessageRepository
    {

        private readonly ChatDbContext _context;


        public MessageRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetMessageById(Guid messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }
        //public async Task<List<Message>> GetConversationMessages(string currentUser, string userId, DateTime? before, int count, string sort)
        //{
        //    var query = _context.Messages.Where(m => (m.Sender.Trim().ToLower() == currentUser.ToLower() 
        //    && m.Receiver.Trim().ToLower() == userId.ToLower()));

        //    if (before.HasValue)
        //    {
        //        query = await query.Where(m => m.Timestamp < before);
        //    }

        //    if (sort.ToLower() == "desc")
        //    {
        //        query = await query.OrderByDescending(m => m.Timestamp);
        //    }
        //    else
        //    {
        //        query = await query.OrderBy(m => m.Timestamp);
        //    }

        //    return await query.Take(count).ToListAsync();
        //}
        public async Task<List<Message>> GetConversationMessages(string currentUser, string userId, DateTime? before, int count, string sort)
        {
            IQueryable<Message> query = _context.Messages.Where(m => m.Sender.Trim().ToLower() == currentUser.ToLower() &&
                                                                     m.Receiver.Trim().ToLower() == userId.ToLower());

            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp < before.Value);
            }

            if (sort.ToLower() == "desc")
            {
                query = query.OrderByDescending(m => m.Timestamp);
            }
            else
            {
                query = query.OrderBy(m => m.Timestamp);
            }

            List<Message> messages;
            try
            {
                messages = await query.Take(count).ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., logging, error notification)
                throw new Exception("An error occurred while retrieving conversation messages.", ex);
            }

            return messages;
        }


        public async Task<Message> CreateMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMessage(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return false;

            _context.Messages.Remove(message);
            return await _context.SaveChangesAsync() > 0;
        }

    }


}

