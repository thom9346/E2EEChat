using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
        public IEnumerable<UserForReturningDto> Get()
        {
            var users = _userRepository.GetAll();
            return _mapper.Map<IEnumerable<UserForReturningDto>>(users);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult Get(Guid id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            var userForReturn = _mapper.Map<UserForReturningDto>(user);
            return Ok(userForReturn);
        }
    }
}
