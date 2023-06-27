using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chat.DominModel.Model
{
  
    public class Message
    {
        [DisplayName("messageId")]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [DisplayName("SenderId")]
        public string Sender { get; set; }

        [DisplayName("ReceiverId")]
        public string Receiver { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
    }
}
