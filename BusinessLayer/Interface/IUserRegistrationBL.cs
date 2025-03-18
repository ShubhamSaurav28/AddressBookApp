using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserRegistrationBL
    {
        public bool RegisterUser(RegistrationModel registrationDTO);
        public string LoginUser(LoginModel loginDTO);
        public bool ForgotPasswordBL(ForgotPasswordModel forgotPasswordDTO);
        public bool ResetPasswordBL(ResetPasswordModel resetPasswordDTO);
        public Task<IEnumerable<GetAllUserModel>> GetAllUsersBL();

    }
}
