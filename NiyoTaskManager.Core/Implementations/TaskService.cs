using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly UserManager<NiyoUser> _userManager;
        private readonly SignInManager<NiyoUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskService> _logger;
        private readonly NiyoDbContext _context;
        private readonly MappingService _mappingService;
        public TaskService(UserManager<NiyoUser> userManager, IConfiguration configuration, ILogger<TaskService> logger, NiyoDbContext context, SignInManager<NiyoUser> signInManager, MappingService mappingService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _mappingService = mappingService;
        }
        public async Task<TaskBindingDTO> CreateTaskAsync(NewTaskDTO model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId && x.IsDeleted==false);
                if (user != null)
                {
                    var newTask = new NiyoTask()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = model.Title,
                        Description = model.Description,
                        User = user,
                    };
                    await _context.Tasks.AddAsync(newTask);
                    await _context.SaveChangesAsync();

                    return _mappingService.MapTasks(newTask);
                }
                else
                {
                    _logger.LogWarning("This user was not found");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An error has occurred");
                return null;
            }
        }

        public async Task DeleteTaskAsync(string id)
        {
            try
            {
                var task = await _context.Tasks.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                if (task != null)
                {
                    task.IsDeleted = true;
                    task.DateDeleted = DateTime.Now;
                }
                else
                {
                    _logger.LogWarning("This task was not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An error has occurred");
            }
        }

        public async Task<List<TaskBindingDTO>> FetchAllTasksAsync()
        {
            try
            {
                var taskList = new List<TaskBindingDTO>();
                var users = await _context.Users.Where(x => x.IsDeleted == false).ToListAsync();

                var tasks = await _context.Tasks.Include(x => x.User).Where(x => x.IsDeleted == false).ToListAsync();
                foreach (var task in tasks)
                {
                    var eachTask = _mappingService.MapTasks(task);
                    taskList.Add(eachTask);
                }
                return taskList;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An error has occurred");
                return null;
            }
        }

        public async Task<TaskBindingDTO> FetchTaskAsync(string id)
        {
            try
            {
                var task = await _context.Tasks.Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
                if (task != null)
                {    
                    return _mappingService.MapTasks(task);
                }
                else
                {
                    _logger.LogWarning($"This task does not exist");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during the account retrieval process {ex.Message}");
                return null;
            }
        }

        public async Task<TaskBindingDTO> UpdateTaskAsync(TaskUpdateDTO model)
        {
            try
            {
                var task = await _context.Tasks.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == model.Id && x.IsDeleted == false);
                if (task != null)
                {
                    task.Title = model.Title;
                    task.Description = model.Description;
                    task.IsCompleted = model.IsCompleted;
                    task.DateUpdated = DateTime.Now;
                    return _mappingService.MapTasks(task);
                }
                else
                {
                    _logger.LogWarning($"This task does not exist");
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
