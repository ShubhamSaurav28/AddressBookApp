using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.DTO
{
    public class GetAllUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }   
        public string Email { get; set; }
    }
}
