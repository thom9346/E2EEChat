using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class UserForReturningDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PublicKey { get; set; }
        public string SigningPublicKey { get; set; }
    }
}
