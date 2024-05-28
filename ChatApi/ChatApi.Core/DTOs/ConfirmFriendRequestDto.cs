using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class ConfirmFriendRequestDto
    {
        public Guid RequestId { get; set; }
        public string Token { get; set; }
        public string RequesteePublicSigningKey { get; set; }
        public Guid RequesteeId { get; set; }
    }
}
