using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Interfaces
{
    public interface IFriendService
    {
        public Task SendFriendRequest(Guid requesterId, string requesteeEmail);
        public void ConfirmFriendRequest(Guid requestId, string token);
        public string CheckFriendRequestStatus(Guid userId, Guid otherUserId);
    }
}
