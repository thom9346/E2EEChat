using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } //plaintext
        public string Email { get; set; }
        public List<string> ConnectionIds { get; set; }
    }
}
