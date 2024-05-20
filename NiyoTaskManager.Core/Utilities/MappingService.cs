using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NiyoTaskManager.Core.Implementations;
using NiyoTaskManager.Data;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Utilities
{
    public class MappingService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskService> _logger;
        private readonly NiyoDbContext _context;
        public MappingService(IConfiguration configuration, ILogger<TaskService> logger, NiyoDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }
        public UserBindingDTO MapUserToModel(NiyoUser user)
        {
            try
            {
                return new UserBindingDTO()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.Email,
                    Country = user.Country,
                    ProfileImage = user.ProfileImage,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error has occured");
                return null;
            }
        }
        public TaskBindingDTO MapTasks(NiyoTask task)
        {
            try
            {
                return new TaskBindingDTO()
                {
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    User = MapUserToModel(task.User) ?? null,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error has occured");
                return null;
            }
        }
    }
}
