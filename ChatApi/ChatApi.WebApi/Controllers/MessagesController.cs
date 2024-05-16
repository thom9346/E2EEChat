using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Services;
using ChatApi.Infrastructure.Repositories;
using ChatApi.WebApi.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace ChatApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ICryptographicManager _cryptographicManager;

        public MessagesController(
            IRepository<Message> messageRepos,
            IRepository<User> userRepos,
            IMapper mapper,
            IHubContext<ChatHub> hubContext,
            ICryptographicManager cryptographicManager
            )
        {
            _messageRepository = messageRepos ??
                throw new ArgumentNullException(nameof(messageRepos));

            _userRepository = userRepos ??
                throw new ArgumentNullException(nameof(userRepos));

            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));

            _hubContext = hubContext ??
                throw new ArgumentNullException(nameof(hubContext));

            _cryptographicManager = cryptographicManager ??
                throw new ArgumentNullException(nameof(cryptographicManager));
        }
        [HttpGet(Name = "GetMessages")]
        public IActionResult Get()
        {
            var messages = _messageRepository.GetAll();
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(messageDtos);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageDto messageDto)
        {
            if (messageDto == null)
            {
                return BadRequest("MessageDto cannot be null");
            }

            // Validate SenderId and RecipientId
            var sender = _userRepository.Get(messageDto.SenderId);
            var recipient = _userRepository.Get(messageDto.RecipientId);

            if (sender == null)
            {
                return BadRequest("Invalid SenderId");
            }

            if (recipient == null)
            {
                return BadRequest("Invalid RecipientId");
            }
            // Convert the recipient's public key from hex string to byte array
            var recipientPublicKeyBytes = Enumerable.Range(0, recipient.PublicKey.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(recipient.PublicKey.Substring(x, 2), 16))
                .ToArray();

            // Convert the message content from hex string to byte array
            var encryptedMessageBytes = Enumerable.Range(0, messageDto.Content.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(messageDto.Content.Substring(x, 2), 16))
                .ToArray();

            // Decrypt message content using recipient's public key
            var decryptedContentBytes = _cryptographicManager.Decrypt(encryptedMessageBytes, recipientPublicKeyBytes);
            var decryptedContent = Encoding.UTF8.GetString(decryptedContentBytes);


            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = messageDto.SenderId,
                RecipientId = messageDto.RecipientId,
                Content = decryptedContent,
                Timestamp = DateTime.UtcNow
            };

            _messageRepository.Add(message);
            var result = _messageRepository.Save();

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving the message.");
            }

            var messageDtoResult = _mapper.Map<MessageDto>(message);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDtoResult);

            return CreatedAtRoute("GetMessages", new { id = messageDtoResult.MessageId }, messageDtoResult);

        }

        [HttpGet("between/{userId1}/{userId2}")]
        public IActionResult GetMessagesBetweenUsers(Guid userId1, Guid userId2)
        {
            var user1Exists = _userRepository.Get(userId1) != null;
            var user2Exists = _userRepository.Get(userId2) != null;

            if (!user1Exists || !user2Exists)
            {
                return BadRequest("One or both user IDs are invalid.");
            }

            var messages = _messageRepository.GetAll()
                .Where(m => (m.SenderId == userId1 && m.RecipientId == userId2) ||
                            (m.SenderId == userId2 && m.RecipientId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToList();

            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(messageDtos);
        }
    }
}
