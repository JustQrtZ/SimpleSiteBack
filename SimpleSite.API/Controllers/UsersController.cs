using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SimpleSite.API.Services.Abstraction;
using SimpleSite.API.ViewModels;
using SimpleSite.Data.Abstract;

namespace SimpleSite.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UsersController(IUserRepository userRepository, ITokenService tokenService)
        {
            this._userRepository = userRepository;
            this._tokenService = tokenService;
        }
        
        [HttpGet("getAllUsers")][Authorize]
        public ActionResult <List<UserViewModel>> GetAllUsers()
        {
            var allUsers = _userRepository.GetAll().Select(x => new
                {x.Id, x.Username, x.Email, x.RegistrationDate, x.LastLoginDate, x.IsActive});

            List<UserViewModel> usersList = new List<UserViewModel>();
            foreach (var variable in allUsers)
            {
               usersList.Add(new UserViewModel(variable.Id, variable.Email, variable.Username,
                   variable.RegistrationDate, variable.LastLoginDate, variable.IsActive));
            }
            
            return usersList;
        }

        [HttpPost("blockUsers")][Authorize]
        public ActionResult BlockUsers([FromBody] string[] usersId)
        {
            var usersListToBlock = _userRepository.FindBy(user => usersId.Contains(user.Id));
            foreach (var user in usersListToBlock)
            {
                user.IsActive = false;
                user.RefreshToken = null;
                _userRepository.Update(user);
            }
            
            _userRepository.Commit();

            return Ok();
        }
        
        [HttpPost("unBlockUsers")][Authorize]
        public ActionResult UnBlockUsers([FromBody] string[] usersId)
        {
            var usersListToUnBlock = _userRepository.FindBy(user => usersId.Contains(user.Id));
            foreach (var user in usersListToUnBlock)
            {
                user.IsActive = true;
                _userRepository.Update(user);
            }
            _userRepository.Commit();
            return Ok();
        }
        
        [HttpPost("deleteUsers")][Authorize]
        public ActionResult DeleteUsers([FromBody] string[] usersId)
        {
            _userRepository.DeleteWhere(user => usersId.Contains(user.Id));
            _userRepository.Commit();
            return Ok();
        }
        
        [HttpPost("getLogenInUser")][Authorize]
        public ActionResult getLogenInUser()
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var user = _userRepository.GetSingle(user => user.Email == principal.Identity.Name);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.Username
                }
            );
        }
        
    }
}