﻿using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.WebApi.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApi.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IFriendService _friendService;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(
            IRepository<Message> messageRepos,
            IRepository<User> userRepos,
            IFriendService friendService,
            IMapper mapper,
            IHubContext<ChatHub> hubContext
            )
        {
            _messageRepository = messageRepos ??
                throw new ArgumentNullException(nameof(messageRepos));

            _userRepository = userRepos ??
                throw new ArgumentNullException(nameof(userRepos));

            _friendService = friendService ??
                throw new ArgumentNullException(nameof(friendService));


            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));

            _hubContext = hubContext ??
                throw new ArgumentNullException(nameof(hubContext));
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

            var senderExists = _userRepository.Get(messageDto.SenderId) != null;
            var recipientExists = _userRepository.Get(messageDto.RecipientId) != null;

            if (!senderExists)
            {
                return BadRequest("Invalid SenderId");
            }

            if (!recipientExists)
            {
                return BadRequest("Invalid RecipientId");
            }
            var checkFriends = _friendService.CheckFriendRequestStatus(messageDto.SenderId, messageDto.RecipientId);
            if (checkFriends != "friends")
            {
                return Forbid("You can only send messages to friends.");
            }

            var message = _mapper.Map<Message>(messageDto);
            message.MessageId = Guid.NewGuid();
            message.Signature = messageDto.Signature;

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

            var messageDtos = messages.Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                RecipientId = m.RecipientId,
                Signature = m.Signature
            });

            return Ok(messageDtos);
        }
    }
}
