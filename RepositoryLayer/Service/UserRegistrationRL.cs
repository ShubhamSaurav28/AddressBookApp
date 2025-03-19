using ModelLayer.DTO;
using System.Linq;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepositoryLayer.Service
{
    public class UserRegistrationRL : IUserRegistrationRL
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IDatabase _redisDb;

        public UserRegistrationRL(ApplicationDbContext context, IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _context = context;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _redisDb = redis.GetDatabase(); // Get Redis database instance
        }

        public bool RegisterUser(RegistrationDTO registrationDTO)
        {
            if (_context.Users.Any(u => u.Email == registrationDTO.Email))
                return false;

            var user = new UserEntity
            {
                Username = registrationDTO.Username,
                Email = registrationDTO.Email,
                PasswordHash = registrationDTO.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public UserEntity LoginUser(LoginDTO loginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDTO.Email);
            if (user == null)
                return null;

            return user;
        }
        public UserEntity GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return null;

            return user;
        }
        public bool UpdateUser(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == resetPasswordDTO.Email);

                if (existingUser == null)
                {
                    return false;
                }

                existingUser.PasswordHash = resetPasswordDTO.NewPassword;

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public bool SaveResetToken(string email, string token)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return false;
                }

                user.ResetToken = token;
                user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reset token: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<UserEntity>> GetAllUserRL()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                return await connection.QueryAsync<UserEntity>("SELECT * FROM Users");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
