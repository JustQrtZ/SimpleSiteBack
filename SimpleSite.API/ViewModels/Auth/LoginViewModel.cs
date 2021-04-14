using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleSite.API.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        
        public DateTime LastLoginDate { get; set; }
    }
}