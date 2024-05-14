using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.DTOs
{
    public class TokenDto
    {
        public string Jwt { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }
}
