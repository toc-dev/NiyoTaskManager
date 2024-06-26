﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NiyoTaskManager.Core.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<NiyoUser> _userManager;
        private readonly SignInManager<NiyoUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly NiyoDbContext _context;
        private readonly IMappingService _mappingService;
        public UserService(UserManager<NiyoUser> userManager, IConfiguration configuration, ILogger<UserService> logger, NiyoDbContext context, SignInManager<NiyoUser> signInManager, IMappingService mappingService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _mappingService = mappingService;
        }
        public async Task<SignInResultDTO> SignUpAsync(SignUpDTO model)
        {
            try
            {
                bool userExists = await _userManager.FindByEmailAsync(model.Email) != null;
                var errors = new List<string>();
                if (!userExists)
                {
                    var niyoUser = new NiyoUser()
                    {
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.Email,
                        Country = model.Country,
                        DateCreated = DateTime.Now,
                        ProfileImage = model.ProfileImage,
                    };
                    var createdUser = await _userManager.CreateAsync(niyoUser, model.Password);
                    if (createdUser.Succeeded)
                    {
                        return await SignInAsync(new SignInDTO()
                        {
                            Email = model.Email,
                            Password = model.Password,
                        });
                    }
                    else
                    {
                        errors.AddRange(createdUser.Errors.Select(x => $"{x.Code} - {x.Description}"));
                    }
                }
                else
                {
                    errors.Add("This user account already exists, please sign-in instead or reset your password");
                }
                return new SignInResultDTO()
                {
                    Errors = errors,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during the signup process");
                return new SignInResultDTO()
                {
                    Errors = new List<string>() { ex.Message }
                };
            }

        }
        public async Task<SignInResultDTO> SignInAsync(SignInDTO model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
               
                if (user != null && user.IsDeleted == true)
                {
                    _logger.LogWarning($"Password sign-in failed for: [{model.Email}] User account closed or deleted");
                    return new SignInResultDTO()
                    {
                        Token = string.Empty,
                        Errors = new List<string> { "User account has been deleted" },
                        User = new UserBindingDTO()
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            PhoneNumber = user.PhoneNumber,
                            UserName = user.Email,
                            Country = user.Country,
                            ProfileImage = user.ProfileImage,
                        }
                    };
                }
                if (user != null && !user.EmailConfirmed)
                {

                    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (signInResult.Succeeded)
                    {
                        _logger.LogWarning($"Sign-in failed for: [{model.Email}] User account has not been confirmed");
                        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("JWTSettings")["Key"]));




                        var now = DateTime.UtcNow;
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokeOptions = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id) }),
                            Expires = now.AddMonths(3), // Set the token to expire in 30 minutes
                            NotBefore = now,
                            Issuer = _configuration.GetSection("JWTSettings")["Issuer"],
                            Audience = _configuration.GetSection("JWTSettings")["Audience"],
                            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokeOptions);
                        var tokenString = tokenHandler.WriteToken(token);

                        return new SignInResultDTO()
                        {
                            Token = tokenString,
                            Expiry = tokeOptions.Expires,
                            User = _mappingService.MapUserToModel(user)
                        };
                    }
                }
                if (user != null)
                {
                    var _signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (_signInResult.Succeeded)
                    {
                        _logger.LogInformation("Password sign-in successful. About to generate authetication token");
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTSettings")["Key"]));




                        var now = DateTime.UtcNow;
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokeOptions = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id) }),
                            Expires = now.AddMonths(3), // Set the token to expire in 30 minutes
                            NotBefore = now,
                            Issuer = _configuration.GetSection("JWTSettings")["Issuer"],
                            Audience = _configuration.GetSection("JWTSettings")["Audience"],
                            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokeOptions);
                        var tokenString = tokenHandler.WriteToken(token);


                        _logger.LogInformation("Token generation successful.");
                        var userViewModel = _mappingService.MapUserToModel(user);
                        return new SignInResultDTO()
                        {
                            Token = tokenString,
                            User = userViewModel,
                            Expiry = tokeOptions.Expires
                        };
                    }
                    else if (_signInResult.IsLockedOut)
                    {
                        _logger.LogWarning($"User with email: [{model.Email}] has been locked out.");
                        return new SignInResultDTO() { Token = string.Empty, Errors = new List<string> { "Your account has been locked." } };
                    }
                    else
                    {
                        _logger.LogWarning($"Password sign-in failed for: [{model.Email}] password is incorrect");
                        return new SignInResultDTO() { Token = string.Empty, Errors = new List<string> { "Your password or email is incorrect" } };
                    }
                }
                else
                {
                    _logger.LogWarning($"Password sign-in failed for: [{model.Email}] Email is incorrect");
                    return new SignInResultDTO() { Token = string.Empty, Errors = new List<string> { "Your password or email is incorrect" } };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during the sign-in process {ex.Message}");
                return new SignInResultDTO() { Token = string.Empty, Errors = new List<string> { ex.Message } };
            }
        }

        public async Task DeleteAccountAsync(string id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user != null)
                {
                    user.IsDeleted = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured during the deletion process {ex.Message}");
            }
        }

        public async Task<List<UserBindingDTO>> FetchAllUsers()
        {
            try
            {
                var userList = new List<UserBindingDTO>();
                var users = await _context.Users.Where(x=>x.IsDeleted==false).ToListAsync();
                foreach (var user in users)
                {
                    var eachUser = _mappingService.MapUserToModel(user);
                    userList.Add(eachUser);
                }
                return userList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during the deletion process {ex.Message}");
                return [];
            }
        }

        public async Task<UserBindingDTO> FetchUserAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.IsDeleted==false);
                if(user != null)
                {
                    _logger.LogWarning($"The user with the email {email} does not exist");
                    return _mappingService.MapUserToModel(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during the account retrieval process {ex.Message}");
                return null;
            }
        }
    }
}

