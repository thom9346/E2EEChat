using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IRepository<User> userRepos, IMapper mapper)
        {
            _userRepository = userRepos ??
                throw new ArgumentNullException(nameof(userRepos));

            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet(Name = "GetUsers")]
        public IActionResult Get()
        {
            var users = _userRepository.GetAll();
            var userDtos = users.Select(user => new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                PublicKey = user.PublicKey // Include public key
            }).ToList();

            return Ok(userDtos);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest();
            }
            User user = _mapper.Map<User>(userDto);

            user.PasswordHash = PasswordHasher.HashPassword(userDto.Password);

            _userRepository.Add(user);
            _userRepository.Save();

            var userDtoResult = _mapper.Map<UserDto>(user);
            userDtoResult.Password = null;

            return CreatedAtRoute("GetUsers", new { id = userDtoResult.UserId }, userDtoResult);

        }
    }
}
