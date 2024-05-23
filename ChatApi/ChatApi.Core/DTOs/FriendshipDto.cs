

namespace ChatApi.Core.DTOs
{
    public class FriendshipDto
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
