using System;

namespace SimpleSite.API.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        
        public bool IsActive { get; set; }

        public UserViewModel(string id, string email, string username, DateTime registrationDate,
            DateTime lastLoginDate, bool isActive)
        {
            this.Id = id;
            this.Email = email;
            this.Username = username;
            this.RegistrationDate = registrationDate;
            this.LastLoginDate = lastLoginDate;
            this.IsActive = isActive;
        }
    }
    
}