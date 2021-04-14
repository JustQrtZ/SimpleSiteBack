using System;
using System.Collections.Generic;
using System.Security.Claims;
using SimpleSite.API.Services.Abstraction;
using SimpleSite.API.ViewModels;
using SimpleSite.Data.Abstract;
using SimpleSite.Model.Entities;
using Microsoft.AspNetCore.Mvc;

namespace SimpleSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        
        public AuthController(ITokenService tokenService, IUserRepository userRepository)
        {
            this._tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService)); 
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)) ;
        }

        [HttpPost, Route("login")]
        public ActionResult Post([FromBody] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _userRepository.GetSingle(u => u.Email == loginModel.Email);

            if (user == null)
            {
                return BadRequest(new { email = "no user with this email" });
            }

            var passwordValid = _tokenService.VerifyPassword(loginModel.Password, user.Password);

            if (!passwordValid)
            {
                return BadRequest(new { password = "invalid password" });
            }

            if (!user.IsActive)
            {
                return BadRequest(new { password = "User is blocked and can't login"});
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginModel.Email),
            };
            
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            user.LastLoginDate = DateTime.Now;
            
            _userRepository.Update(user);
            _userRepository.Commit();
            
            return Ok( new
            {
                user.Id,
                user.Username,
                accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("register")]
        public ActionResult Post([FromBody] RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var emailUniq = _userRepository.IsEmailUniq(registerModel.Email);
            if (!emailUniq) return BadRequest(new { email = "user with this email already exists" });
            var usernameUniq = _userRepository.IsUsernameUniq(registerModel.Username);
            if (!usernameUniq) return BadRequest(new { username = "user with this email already exists" });

            var id = Guid.NewGuid().ToString();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, registerModel.Email),
            };
            
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            var user = new User
            {
                Id = id,
                Username = registerModel.Username,
                Email = registerModel.Email,
                Password = _tokenService.HashPassword(registerModel.Password),
                RefreshToken = _tokenService.GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                RegistrationDate = DateTime.Now,
                LastLoginDate = DateTime.Now
            };
            
            _userRepository.Add(user);
            _userRepository.Commit();

            return Ok( new
            {
                user.Id,
                user.Username,
                accessToken,
                RefreshToken = refreshToken
            });
        }

    }
}