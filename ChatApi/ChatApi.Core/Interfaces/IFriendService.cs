using ChatApi.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Interfaces
{
    public interface IFriendService
    {
        public Task SendFriendRequest(SendFriendRequestDto requestDto);
        public void ConfirmFriendRequest(ConfirmFriendRequestDto requestDto);
        public string CheckFriendRequestStatus(Guid userId, Guid otherUserId);
    }
}
