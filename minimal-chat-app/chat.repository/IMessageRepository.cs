using Chat.DominModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Repository
{
    public interface IMessageRepository
    {
        Task<Message> GetMessageById(Guid messageId);
        Task<Message> CreateMessage(Message message);
        Task<bool> UpdateMessage(Message message);
        Task<bool> DeleteMessage(Guid messageId);
        Task<List<Message>> GetConversationMessages(string currentUser, string userId, DateTime? before, int count, string sort);
    }

}
