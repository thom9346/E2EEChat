using AutoMapper;
using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ICryptographicManager _cryptographicManager;

        public AuthController(IRepository<User> userRepository, IMapper mapper, IConfiguration configuration, ICryptographicManager cryptographicManager)
        {
            _userRepository = userRepository 
                ?? throw new ArgumentNullException(nameof(userRepository));

            _mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));

            _configuration = configuration 
                ?? throw new ArgumentNullException(nameof(configuration));

            _cryptographicManager = cryptographicManager
                ?? throw new ArgumentNullException(nameof(cryptographicManager));
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null) return BadRequest("Invalid client request");

            var existingUser = _userRepository.GetAll().FirstOrDefault(u => u.Username == registerDto.Username);
            if (existingUser != null) return BadRequest("Username is already taken");

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = PasswordHasher.HashPassword(registerDto.Password);
            user.PublicKey = registerDto.PublicKey;
            user.SigningPublicKey = registerDto.SigningPublicKey;

            byte[] emailBytes = Encoding.UTF8.GetBytes(registerDto.Email);
            byte[] iv;
            byte[] encryptedEmailBytes = _cryptographicManager.Encrypt(emailBytes, out iv);
            user.Email = Convert.ToBase64String(encryptedEmailBytes) + "." + Convert.ToBase64String(iv);

            _userRepository.Add(user);
            if (!_userRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null) return BadRequest("Invalid client request");

            var user = _userRepository.GetAll().FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);

            var tokenDto = new TokenDto
            {
                Jwt = token,
                Message = "Login successful",
                UserId = user.UserId
            };
            return Ok(tokenDto);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
