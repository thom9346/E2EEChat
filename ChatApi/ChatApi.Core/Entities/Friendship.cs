using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Entities
{
    public class Friendship
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid RequesteeId { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string VerificationToken { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
