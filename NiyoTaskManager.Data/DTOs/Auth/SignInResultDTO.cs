using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.Auth
{
    public class SignInResultDTO
    {
        public List<string>? Errors { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiry { get; set; }
        public UserBindingDTO? User { get; set; }
    }
}
