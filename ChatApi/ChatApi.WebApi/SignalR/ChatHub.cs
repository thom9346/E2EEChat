using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatApi.WebApi.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Connection> _connectionRepository;

        public ChatHub(IRepository<User> userRepository, IRepository<Connection> connectionRepository)
        {
            _userRepository = userRepository;
            _connectionRepository = connectionRepository;
        }

        public async Task SendPublicKey(string publicKey, Guid recipientUserId)
        {
            var recipient = _userRepository.Get(recipientUserId);
            if (recipient?.Connections != null)
            {
                foreach (var connection in recipient.Connections)
                {
                    await Clients.Client(connection.ConnectionString).SendAsync("ReceivePublicKey", publicKey);
                }
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // Assuming user identifier is set
            if (userId != null)
            {
                var user = _userRepository.Get(Guid.Parse(userId));
                if (user != null)
                {
                    var connection = new Connection
                    {
                        ConnectionId = Guid.NewGuid(),
                        ConnectionString = Context.ConnectionId,
                        UserId = user.UserId
                    };
                    user.Connections.Add(connection);
                    _connectionRepository.Add(connection);
                    _userRepository.Save();
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = _connectionRepository.Get(Context.ConnectionId);
            if (connection != null)
            {
                _connectionRepository.Remove(connection.ConnectionId);
                _connectionRepository.Save();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
