﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.Auth
{
    public class SignInDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
