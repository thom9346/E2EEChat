using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendService _friendService;
        private readonly IMapper _mapper;

        public FriendshipController(IFriendService friendService, IMapper mapper)
        {
            _friendService = friendService;
            _mapper = mapper;
        }

        [HttpPost("send-friend-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestDto requestDto)
        {
            try
            {
                await _friendService.SendFriendRequest(requestDto.RequesterId, requestDto.RequesteeEmail);
                return Ok("Friend request sent.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify-friend-request")]
        public IActionResult VerifyFriendRequest(Guid requestId, string token)
        {
            try
            {
                _friendService.ConfirmFriendRequest(requestId, token);
                return Ok("Friend request confirmed.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
