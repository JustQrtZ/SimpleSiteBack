using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleSite.API.Services.Abstraction
{
    public interface ITokenService
    {
        string HashPassword(string password);
        bool VerifyPassword(string actualPassword, string hashedPassword);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
