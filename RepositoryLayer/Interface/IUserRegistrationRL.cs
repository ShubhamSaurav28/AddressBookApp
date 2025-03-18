using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRegistrationRL
    {
        public bool RegisterUser(RegistrationModel registrationDTO);
        public UserEntity LoginUser(LoginModel loginDTO);
        public UserEntity GetUserByEmail(string email);
        public bool UpdateUser(ResetPasswordModel resetPasswordDTO);
        public bool SaveResetToken(string email, string token);
        public Task<IEnumerable<UserEntity>> GetAllUserRL();

    }
}
