using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DominModel.Model
{
    public class Message
    {
        public string MessageId { get; set; }

        public string SenderId { get; set; }
        public User User1 { get; set; }

        public string ReceiverId { get; set; }
        public User User2 { get; set; }

        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
