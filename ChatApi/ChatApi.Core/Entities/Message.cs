using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Entities
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }
}
