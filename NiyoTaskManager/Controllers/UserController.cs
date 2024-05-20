using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NiyoTaskManager.Core.Implementations;
using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.Entities;

namespace NiyoTaskManager.API.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        protected readonly SignInManager<NiyoUser> _signInManager;
        public UserController(IUserService userService, SignInManager<NiyoUser> signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpPost("account/signin")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO model)
        {
            try
            {
                var response = await _userService.SignInAsync(model);
                if (!string.IsNullOrEmpty(response.Token))
                {
                    return Ok(new NiyoAPICustomResponseSchema("Login Successful", response));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(response.Errors, response));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }

        [AllowAnonymous]
        [HttpPost("account/signup")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO model)
        {
            try
            {
                var response = await _userService.SignUpAsync(model);
                if (response.Errors != null)
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(response.Errors, response));
                }
                else
                {
                    return Ok(new NiyoAPICustomResponseSchema("Sign Up Successful", response));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        [AllowAnonymous]
        [HttpPost("account/allusers")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> FetchAllUsers()
        {
            try
            {
                var response = await _userService.FetchAllUsers();
                return Ok(new NiyoAPICustomResponseSchema("Users successfully fetched", response));
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        
        [AllowAnonymous]
        [HttpPost("account/user")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> FetchUser(string id)
        {
            try
            {
                var response = await _userService.FetchUserAsync(id);
                return Ok(new NiyoAPICustomResponseSchema("User successfully fetched", response));
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }

    }
}
