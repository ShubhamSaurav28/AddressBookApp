using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTO
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[A-Za-z0-9]{5,15}$", ErrorMessage = "Username must be 5-15 characters long and contain only letters and numbers.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long, including 1 uppercase letter, 1 number, and 1 special character.")]
        public string Password { get; set; }
    }
}
