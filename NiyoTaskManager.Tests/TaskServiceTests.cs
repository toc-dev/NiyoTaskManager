using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NiyoTaskManager.Core.Implementations;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data.Entities;
using NiyoTaskManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using Microsoft.Extensions.Options;
using NiyoTaskManager.Core.Interfaces;

namespace NiyoTaskManager.Tests
{
    public class TaskServiceTests : IDisposable
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;
        private readonly NiyoDbContext _context;
        private readonly Mock<IMappingService> _mappingServiceMock;
        private readonly ITaskService _taskService;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<NiyoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new NiyoDbContext(options);

            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<TaskService>>();
            _mappingServiceMock = new Mock<IMappingService>();

            _taskService = new TaskService(_configurationMock.Object, _loggerMock.Object, _context, _mappingServiceMock.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        
        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTask_WhenUserExists()
        {
            // Arrange
            var userId = "user-id-1";
            var user = new NiyoUser { Id = userId, Email = "test@example.com", IsDeleted = false };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var newTaskDto = new NewTaskDTO { Title = "New Task", Description = "Task Description", UserId = userId };

            _mappingServiceMock.Setup(ms => ms.MapTasks(It.IsAny<NiyoTask>())).Returns(new TaskBindingDTO { Title = newTaskDto.Title });

            // Act
            var result = await _taskService.CreateTaskAsync(newTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newTaskDto.Title, result.Title);
            var createdTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Title == newTaskDto.Title);
            Assert.NotNull(createdTask);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var newTaskDto = new NewTaskDTO { Title = "New Task", Description = "Task Description", UserId = "non-existent-user-id" };

            // Act
            var result = await _taskService.CreateTaskAsync(newTaskDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
        {
            // Arrange
            var userId = "user-id-1";
            var user = new NiyoUser { Id = userId, Email = "test@example.com", IsDeleted = false };
            var task = new NiyoTask { Id = "task-id-1", Title = "Old Title", Description = "Old Description", User = user, IsDeleted = false };
            await _context.Users.AddAsync(user);
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            var updateTaskDto = new TaskUpdateDTO { Id = task.Id, Title = "New Title", Description = "New Description", IsCompleted = true };

            _mappingServiceMock.Setup(ms => ms.MapTasks(It.IsAny<NiyoTask>())).Returns(new TaskBindingDTO { Title = updateTaskDto.Title });

            // Act
            var result = await _taskService.UpdateTaskAsync(updateTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateTaskDto.Title, result.Title);
            var updatedTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal(updateTaskDto.Title, updatedTask.Title);
            Assert.Equal(updateTaskDto.Description, updatedTask.Description);
            Assert.True(updatedTask.IsCompleted);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var updateTaskDto = new TaskUpdateDTO { Id = "non-existent-task-id", Title = "New Title", Description = "New Description", IsCompleted = true };

            // Act
            var result = await _taskService.UpdateTaskAsync(updateTaskDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FetchAllTasksAsync_ShouldReturnListOfTasks()
        {
            // Arrange
            var userId = "user-id-1";
            var user = new NiyoUser { Id = userId, Email = "test@example.com", IsDeleted = false };
            var task1 = new NiyoTask { Id = "task-id-1", Title = "Task 1", Description = "Description 1", User = user, IsDeleted = false };
            var task2 = new NiyoTask { Id = "task-id-2", Title = "Task 2", Description = "Description 2", User = user, IsDeleted = false };
            await _context.Users.AddAsync(user);
            await _context.Tasks.AddRangeAsync(task1, task2);
            await _context.SaveChangesAsync();

            _mappingServiceMock.Setup(ms => ms.MapTasks(It.IsAny<NiyoTask>()))
                .Returns<NiyoTask>(t => new TaskBindingDTO { Title = t.Title });

            // Act
            var result = await _taskService.FetchAllTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Title == task1.Title);
            Assert.Contains(result, r => r.Title == task2.Title);
        }

        [Fact]
        public async Task FetchTaskAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            var userId = "user-id-1";
            var user = new NiyoUser { Id = userId, Email = "test@example.com", IsDeleted = false };
            var task = new NiyoTask { Id = "task-id-1", Title = "Task Title", Description = "Task Description", User = user, IsDeleted = false };
            await _context.Users.AddAsync(user);
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            _mappingServiceMock.Setup(ms => ms.MapTasks(It.IsAny<NiyoTask>())).Returns(new TaskBindingDTO { Title = task.Title });

            // Act
            var result = await _taskService.FetchTaskAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Title, result.Title);
        }

        [Fact]
        public async Task FetchTaskAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var nonExistentTaskId = "non-existent-task-id";

            // Act
            var result = await _taskService.FetchTaskAsync(nonExistentTaskId);

            // Assert
            Assert.Null(result);
        }


    }
}
