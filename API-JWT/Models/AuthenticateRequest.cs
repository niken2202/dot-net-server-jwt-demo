﻿using System;
using System.ComponentModel.DataAnnotations;

namespace API_JWT.Models
{
      public class AuthenticateRequest
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }
    
}
