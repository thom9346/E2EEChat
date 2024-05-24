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

        public FriendshipController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("send-friend-request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestDto requestDto)
        {
            try
            {
                await _friendService.SendFriendRequest(requestDto.RequesterId, requestDto.RequesteeEmail);
                return Ok(new { message = "Friend request sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("confirm-friend-request")]
        public IActionResult ConfirmFriendRequest([FromBody] ConfirmFriendRequestDto requestDto)
        {
            try
            {
                _friendService.ConfirmFriendRequest(requestDto.RequestId, requestDto.Token);
                return Ok(new { message = "Friend request confirmed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("check-friend-request-status/{userId}/{otherUserId}")]
        public IActionResult CheckFriendRequestStatus(Guid userId, Guid otherUserId)
        {
            try
            {
                var status = _friendService.CheckFriendRequestStatus(userId, otherUserId);
                return Ok(new { status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
