using System;

namespace SimpleSite.Model.Entities
{
    public class User : IEntityBase
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        
        public string RefreshToken { get; set; }
        
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = true;
        

    }
}