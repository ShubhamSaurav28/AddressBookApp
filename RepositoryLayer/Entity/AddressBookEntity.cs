using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, Phone, MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(250)]
        public string Address { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        // Foreign Key
        
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        // Navigation Property (nullable to avoid circular reference issues)
        public UserEntity? User { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
