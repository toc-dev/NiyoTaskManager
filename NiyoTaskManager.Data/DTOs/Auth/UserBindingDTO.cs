using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.Auth
{
    public class UserBindingDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Country { get; set; }
        public string? ProfileImage { get; set; }
    }
}
