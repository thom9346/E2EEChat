using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class SendFriendRequestDto
    {
        public Guid RequesterId { get; set; }
        public string RequesteeEmail { get; set; }
    }
}
