using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Service;
using ModelLayer.DTO;
using BusinessLayer.Interface;
using ModelLayer.Models;

namespace UserRegistration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistrationBL _userRegistrationBL;
        private readonly ILogger<UserRegistrationController> _logger;

        public UserRegistrationController(IUserRegistrationBL userRegistrationBL, ILogger<UserRegistrationController> logger)
        {
            _userRegistrationBL = userRegistrationBL;
            _logger = logger;
        }



        [HttpPost("register")]
        public IActionResult RegisterUser(RegistrationDTO registrationDTO)
        {
            try
            {
                _logger.LogInformation("RegisterUser() called for {Email}", registrationDTO.Email);

                bool result = _userRegistrationBL.RegisterUser(registrationDTO);

                if (result)
                {
                    return Ok(new { Success = true, Message = "User Registered Successfully!" });
                }

                return Conflict(new { Success = false, Message = "User Already Exists!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterUser()");
                return BadRequest(new { Success = false, Message = "An error occurred during registration." });
            }
        }


        [HttpPost("login")]
        public IActionResult LoginUser(LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation("LoginUser() called for {Username}", loginDTO.Email);

                string result = _userRegistrationBL.LoginUser(loginDTO);

                if (result!=null)
                {   
                    return Ok(new { Success = true, Message = "Login Successful", Data = result });
                }

                return Unauthorized(new { Success = false, Message = "Invalid Credentials", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginUser()");
                return BadRequest(new { Success = false, Message = "An error occurred during login." });
            }
        }


        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                _logger.LogInformation("ForgotPassword() called for {Email}", forgotPasswordDTO.Email);

                bool result = _userRegistrationBL.ForgotPasswordBL(forgotPasswordDTO);
                if (result)
                {
                    return Ok(new { Success = true, Message = "Reset password link sent to email." });
                }

                return NotFound(new { Success = false, Message = "User not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword()");
                return BadRequest(new { Success = false, Message = "An error occurred while processing the request." });
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                _logger.LogInformation("ResetPassword() called for token: {Token}", resetPasswordDTO.Token);

                bool result = _userRegistrationBL.ResetPasswordBL(resetPasswordDTO);
                if (result)
                {
                    return Ok(new { Success = true, Message = "Password reset successful." });
                }

                return BadRequest(new { Success = false, Message = "Invalid or expired token." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword()");
                return BadRequest(new { Success = false, Message = "An error occurred while resetting the password." });
            }
        }

        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                _logger.LogInformation("GetAllUsers() called.");
                var users = _userRegistrationBL.GetAllUsersBL();

                if (users == null)
                {

                    return NotFound(new { Success = false, Message = "No users found." });
                }

                return Ok(new { Success = true, Message = "Users retrieved successfully.", Data = users.Result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllUsers()");
                return BadRequest(new { Success = false, Message = "An error occurred while fetching users." });
            }
        }


    }
}
