﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DominModel.Model
{
    public class User
    {
        
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,}$", ErrorMessage = "Password must be alphanumeric including at least 1 uppercase letter,1 lowercase letter with minimum 8 characters.")]
        public string Password { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
