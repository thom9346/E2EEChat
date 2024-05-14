using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(IRepository<Message> messageRepos, IRepository<User> userRepos, IMapper mapper)
        {
            _messageRepository = messageRepos ??
                throw new ArgumentNullException(nameof(messageRepos));

            _userRepository = userRepos ??
                throw new ArgumentNullException(nameof(userRepos));

            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet(Name = "GetMessages")]
        public IActionResult Get()
        {
            var messages = _messageRepository.GetAll();
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(messageDtos);
        }
        [HttpPost]
        public IActionResult Post([FromBody] MessageDto messageDto)
        {
            if (messageDto == null)
            {
                return BadRequest("MessageDto cannot be null");
            }

            // Validate SenderId and RecipientId
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

            var message = _mapper.Map<Message>(messageDto);
            message.MessageId = Guid.NewGuid();
            _messageRepository.Add(message);
            var result = _messageRepository.Save();

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving the message.");
            }

            var messageDtoResult = _mapper.Map<MessageDto>(message);

            return CreatedAtRoute("GetMessages", new { id = messageDtoResult.MessageId }, messageDtoResult);

        }
    }
}
