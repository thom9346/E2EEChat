﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class MessageDto
    {
        public Guid? MessageId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Signature { get; set; }
    }
}
