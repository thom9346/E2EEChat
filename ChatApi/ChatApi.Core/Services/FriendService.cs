using ChatApi.Core.DTOs;
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Services
{
    public class FriendService : IFriendService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Friendship> _friendshipRepository;
        private readonly ICryptographicManager _cryptographicManager;
        private readonly IEmailService _emailService;

        public FriendService(IRepository<User> userRepository, IRepository<Friendship> friendshipRepository, IEmailService emailService, ICryptographicManager cryptographicManager)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _emailService = emailService;
            _cryptographicManager = cryptographicManager;
        }
        public async Task SendFriendRequest(SendFriendRequestDto requestDto)
        {
            var requestee = _userRepository.GetAll().FirstOrDefault(u => u.Email == requestDto.RequesteeEmail);
            var requester = _userRepository.Get(requestDto.RequesterId);
            var requesterUserNameLowercase = requester.Username.ToLower();
   
            if (requestee == null)
            {
                throw new ArgumentException("User with the provided email does not exist.");
            }
            var emailParts = requestee.Email.Split('.');
            if (emailParts.Length != 2)
            {
                throw new ArgumentException("Invalid token format.");
            }

            var token = GenerateToken();

            byte[] iv;
            var encryptedToken = _cryptographicManager.Encrypt(Encoding.UTF8.GetBytes(token), out iv);

            var friendship = new Friendship
            {
                Id = Guid.NewGuid(),
                RequesterId = requestDto.RequesterId,
                RequesteeId = requestee.UserId,
                IsConfirmed = false,
                RequestedAt = DateTime.UtcNow,
                VerificationToken = Convert.ToBase64String(encryptedToken) + "." + Convert.ToBase64String(iv),
                TokenExpiration = DateTime.UtcNow.AddHours(24)
            };


            _friendshipRepository.Add(friendship);
            _friendshipRepository.Save();


        

            var encryptedEmail = Convert.FromBase64String(emailParts[0]);
            var Emailiv = Convert.FromBase64String(emailParts[1]);

            var decryptedEmail = Encoding.UTF8.GetString(_cryptographicManager.Decrypt(encryptedEmail, Emailiv));

            var verificationUrl = $"http://localhost:4200/verify-friend-request?requestId={friendship.Id}&token={token}&requesterSigningPublicKey={requestDto.RequesterSigningPublicKey}&requesteeId={requestee.UserId}";
            await _emailService.SendEmailAsync(decryptedEmail, "Friend Request Verification", $"You just got a friend request from: {requesterUserNameLowercase}. (Note that all usernames are shown in all lowercase letters). Please verify your friend request by clicking <a href='{verificationUrl}'>here</a>.");
        }
        private string GenerateToken()
        {
            return Convert.ToBase64String(_cryptographicManager.CryptographicRandomNumberGenerator(32));
        }

        public void ConfirmFriendRequest(ConfirmFriendRequestDto requestDto)
        {
            var friendship = _friendshipRepository.Get(requestDto.RequestId);
            var user = _userRepository.Get(requestDto.RequesteeId);
            if (friendship == null)
            {
                throw new ArgumentException("Invalid friend request ID.");
            }
            if(user.SigningPublicKey != requestDto.RequesteePublicSigningKey)
            {
                throw new ArgumentException("Public key is incorrect.");
            };
            if (friendship.IsConfirmed) 
            {
                throw new ArgumentException("You are already friends with this user.");
            }

            if (DateTime.UtcNow > friendship.TokenExpiration)
            {
                throw new ArgumentException("Token has expired.");
            }
            var tokenParts = friendship.VerificationToken.Split('.');
            if (tokenParts.Length != 2)
            {
                throw new ArgumentException("Invalid token format.");
            }


            var encryptedToken = Convert.FromBase64String(tokenParts[0]);
            var iv = Convert.FromBase64String(tokenParts[1]);

            var decryptedToken = Encoding.UTF8.GetString(_cryptographicManager.Decrypt(encryptedToken, iv));

            //sanitize
            decryptedToken = decryptedToken.Trim();
            var token = requestDto.Token.Trim();

            if (decryptedToken != token)
            {
                throw new ArgumentException("Invalid token.");
            }

            friendship.IsConfirmed = true;
            friendship.ConfirmedAt = DateTime.UtcNow;

            _friendshipRepository.Edit(friendship);
            _friendshipRepository.Save();
        }
        public string CheckFriendRequestStatus(Guid userId, Guid otherUserId)
        {
            var friendship = _friendshipRepository.GetAll().FirstOrDefault(f =>
                (f.RequesterId == userId && f.RequesteeId == otherUserId) ||
                (f.RequesterId == otherUserId && f.RequesteeId == userId));

            if (friendship == null)
            {
                return "no_request";
            }
            else if (friendship.IsConfirmed)
            {
                return "friends";
            }
            else if (friendship.RequesterId == userId)
            {
                return "request_sent";
            }
            else
            {
                return "request_received";
            }
        }
    }
}
