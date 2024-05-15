using ChatApi.Core.DTOs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApi.WebApi.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessageDto message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
