using System.ComponentModel.DataAnnotations;

namespace ModelLayer.DTO
{
    public class RegistrationModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
