using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApi.WebApi.SignalR
{
    public class ChatHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string groupName, MessageDto message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }
    }
}
