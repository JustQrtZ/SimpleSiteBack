using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleSite.API.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 1)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 1)]
        public string Password { get; set; }
        
        [Required]
        public DateTime RegistrationDate { get; set; }
        
        [Required]
        public DateTime LastLoginDate { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
    }
}