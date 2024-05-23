﻿using ChatApi.Core.Entities;
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

        private readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes("your-32-character-key");

        public FriendService(IRepository<User> userRepository, IRepository<Friendship> friendshipRepository, IEmailService emailService, ICryptographicManager cryptographicManager)
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
            _emailService = emailService;
            _cryptographicManager = cryptographicManager;
        }
        public async Task SendFriendRequest(Guid requesterId, string requesteeEmail)
        {
            var requestee = _userRepository.GetAll().FirstOrDefault(u => u.Email == requesteeEmail);
            if (requestee == null)
            {
                throw new ArgumentException("User with the provided email does not exist.");
            }
            var token = GenerateToken();

            byte[] iv;
            var encryptedToken = _cryptographicManager.Encrypt(Encoding.UTF8.GetBytes(token), out iv);

            var friendship = new Friendship
            {
                Id = Guid.NewGuid(),
                RequesterId = requesterId,
                RequesteeId = requestee.UserId,
                IsConfirmed = false,
                RequestedAt = DateTime.UtcNow,
                VerificationToken = Convert.ToBase64String(encryptedToken) + "." + Convert.ToBase64String(iv),
                TokenExpiration = DateTime.UtcNow.AddHours(24)
            };

            _friendshipRepository.Add(friendship);
            _friendshipRepository.Save();

            var verificationUrl = $"http://localhost:4200/verify-friend-request?requestId={friendship.Id}&token={token}";
            await _emailService.SendEmailAsync(requestee.Email, "Friend Request Verification", $"Please verify your friend request by clicking <a href='{verificationUrl}'>here</a>.");
        }
        private string GenerateToken()
        {
            return Convert.ToBase64String(_cryptographicManager.CryptographicRandomNumberGenerator(32));
        }

        public void ConfirmFriendRequest(Guid requestId, string token)
        {
            var friendship = _friendshipRepository.Get(requestId);
            if (friendship == null)
            {
                throw new ArgumentException("Invalid friend request ID.");
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
            if (decryptedToken != token)
            {
                throw new ArgumentException("Invalid token.");
            }

            friendship.IsConfirmed = true;
            friendship.ConfirmedAt = DateTime.UtcNow;

            _friendshipRepository.Edit(friendship);
            _friendshipRepository.Save();
        }
    }
}
