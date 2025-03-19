using System;
using System.Security.Cryptography;
using System.Text;
using BusinessLayer.Interface;
using Middleware;
using ModelLayer.DTO;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using StackExchange.Redis;

namespace BusinessLayer.Service
{
    public class UserRegistrationBL : IUserRegistrationBL
    {
        private readonly IUserRegistrationRL _userRegistrationRL;
        private readonly JwtMiddleware _jwtMiddleware;
        private readonly IEmailServiceBL _emailServiceBL;

        private const int SaltSize = 16; // 128-bit
        private const int HashSize = 32; // 256-bit
        private const int Iterations = 10000; // Recommended for PBKDF2

        public UserRegistrationBL(IUserRegistrationRL userRegistrationRL, JwtMiddleware jwtMiddleware, IEmailServiceBL emailServiceBL)
        {
            _userRegistrationRL = userRegistrationRL;
            _jwtMiddleware = jwtMiddleware;
            _emailServiceBL = emailServiceBL;
        }

        public bool RegisterUser(RegistrationDTO registrationDTO)
        {
            string hashedPassword = HashPassword(registrationDTO.Password);
            registrationDTO.Password = hashedPassword;
            return _userRegistrationRL.RegisterUser(registrationDTO);
        }

        public string LoginUser(LoginDTO loginDTO)
        {

            var user = _userRegistrationRL.LoginUser(loginDTO);

            if (user != null && VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                var token = _jwtMiddleware.GenerateToken(user);

                return token;
            }
            return null;
        }


        public bool ForgotPasswordBL(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = _userRegistrationRL.GetUserByEmail(forgotPasswordDTO.Email);
            if (user == null)
            {
                return false;
            }

            string token = _jwtMiddleware.GenerateResetToken(user.Email);

            bool isTokenSaved = _userRegistrationRL.SaveResetToken(forgotPasswordDTO.Email, token);
            if (!isTokenSaved)
            {
                return false;
            }

            string resetLink = $"https://localhost:7238/reset-password?token={token}";
            string emailBody = $"<h3>Password Reset</h3><p>Click <a href='{resetLink}'>here</a> to reset your password.</p><p>{resetLink}</p>";

            _emailServiceBL.SendEmail(forgotPasswordDTO.Email, "Password Reset Request", emailBody);

            return true;
        }

        public bool ResetPasswordBL(ResetPasswordDTO resetPasswordDTO)
        {
            var result = _jwtMiddleware.ValidateResetToken(resetPasswordDTO);

            if (result == false)
            {
                return false;
            }

            string hashedPassword = HashPassword(resetPasswordDTO.NewPassword);

            resetPasswordDTO.NewPassword = hashedPassword;
            bool isUpdated = _userRegistrationRL.UpdateUser(resetPasswordDTO);

            return isUpdated;
        }


        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
                return Convert.ToBase64String(hashBytes);
            }

        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i])
                        return false;
                }
            }
            return true;

        }

        public async Task<IEnumerable<GetAllUserModel>> GetAllUsersBL()
        {
            try
            {
                var users = await _userRegistrationRL.GetAllUserRL();
                var filteredUsers = users.Select(user => new GetAllUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username
                }).ToList();
                return filteredUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
