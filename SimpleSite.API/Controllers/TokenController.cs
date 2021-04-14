using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using SimpleSite.API.Services.Abstraction;
using SimpleSite.API.ViewModels;
using SimpleSite.Data.Abstract;

namespace SimpleSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        readonly IUserRepository _userContext;
        readonly ITokenService _tokenService;

        public TokenController(IUserRepository userContext, ITokenService tokenService)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiViewModel tokenApiModel)
        {
            Console.WriteLine(tokenApiModel.AccessToken + '\n' + tokenApiModel.RefreshToken);
            try
            {
                if (tokenApiModel is null)
                {
                    return BadRequest("Invalid client request");
                }

                string accessToken = tokenApiModel.AccessToken;
                string refreshToken = tokenApiModel.RefreshToken;
            
                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                var username = principal.Identity?.Name; //this is mapped to the Name claim by default
            
                var user = _userContext.GetSingle(u => refreshToken != null && u.RefreshToken == refreshToken && u.Email == username);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now || !user.IsActive)
                {
                    return BadRequest("Invalid client request");
                }
            
                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                user.RefreshToken = _tokenService.GenerateRefreshToken();

                _userContext.Update(user);
                _userContext.Commit();

                return new ObjectResult(new
                {
                    accessToken = newAccessToken,
                    refreshToken = user.RefreshToken
                });
            }
            catch (Exception e)
            
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var userEmail = User.Identity?.Name;

            var user = _userContext.GetSingle(u => u.Email == userEmail);

            if (user == null) return BadRequest();

            if (user.RefreshToken == null)
            {
                return Unauthorized();
            }
            
            user.RefreshToken = null;

            _userContext.Update(user);
            _userContext.Commit();
            
            return NoContent();
        }

    }
}