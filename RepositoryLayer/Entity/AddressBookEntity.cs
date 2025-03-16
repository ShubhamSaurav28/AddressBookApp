using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        public string? Email { get; set; }

        public int UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
