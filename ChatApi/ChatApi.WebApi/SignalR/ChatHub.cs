using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApi.WebApi.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, byte[] encryptedMessage)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, encryptedMessage);
        }

        public async Task SendPublicKey(string publicKey)
        {
            await Clients.Others.SendAsync("ReceivePublicKey", publicKey);
        }
    }
}
