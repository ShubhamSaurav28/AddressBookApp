using Microsoft.IdentityModel.Tokens;
using System.Text;
using RepositoryLayer.Entity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Interface;

namespace Middleware
{
    public class JwtMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRegistrationRL _userRegistrationRL;
        public JwtMiddleware(IConfiguration configuration, IUserRegistrationRL userRegistrationRL)
        {
            _configuration = configuration;
            _userRegistrationRL = userRegistrationRL;
        }

        public string GenerateToken(UserEntity user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object is null.");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            if (jwtSettings == null)
            {
                throw new Exception("JWT settings are missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("Username", user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateResetToken(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email), "User object is null.");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            if (jwtSettings == null)
            {
                throw new Exception("JWT settings are missing in configuration.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["ResetSecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("email", email),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateResetToken(ResetPasswordModel resetPasswordDTO)
        {
            var user = _userRegistrationRL.GetUserByEmail(resetPasswordDTO.Email);

            if (user == null || user.ResetToken != resetPasswordDTO.Token || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }

            return true;

        }


        //public ClaimsPrincipal? VerifyToken(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return null;
        //    }

        //    var jwtSettings = _configuration.GetSection("Jwt");
        //    if (jwtSettings == null)
        //    {
        //        throw new Exception("JWT settings are missing in configuration.");
        //    }

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = jwtSettings["Issuer"],
        //            ValidAudience = jwtSettings["Audience"],
        //            IssuerSigningKey = securityKey,
        //            ClockSkew = TimeSpan.Zero 
        //        };

        //        SecurityToken validatedToken;
        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        //        return principal;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

    }
}
