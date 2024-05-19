using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.Auth
{
    public class SignUpResultDTO
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
