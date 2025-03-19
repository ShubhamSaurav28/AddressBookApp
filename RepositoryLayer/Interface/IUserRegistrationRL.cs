using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRegistrationRL
    {
        public bool RegisterUser(RegistrationDTO registrationDTO);
        public UserEntity LoginUser(LoginDTO loginDTO);
        public UserEntity GetUserByEmail(string email);
        public bool UpdateUser(ResetPasswordDTO resetPasswordDTO);
        public bool SaveResetToken(string email, string token);
        public Task<IEnumerable<UserEntity>> GetAllUserRL();

    }
}
