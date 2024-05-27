using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NiyoTaskManager.Core.Implementations;
using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly Mock<UserManager<NiyoUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly Mock<IMappingService> _mappingServiceMock;
        private readonly NiyoDbContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<NiyoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new NiyoDbContext(options);

            var userStoreMock = new Mock<IUserStore<NiyoUser>>();
            _userManagerMock = new Mock<UserManager<NiyoUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _mappingServiceMock = new Mock<IMappingService>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private SignInManager<NiyoUser> GetSignInManager()
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<NiyoUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<NiyoUser>>>();

            var signInManager = new SignInManager<NiyoUser>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                optionsMock.Object,
                loggerMock.Object, null
            );

            return signInManager;
        }

        [Fact]
        public async Task SignUpAsync_ShouldCreateUser_WhenUserDoesNotExist()
        {
            var model = new SignUpDTO { Email = "test@example.com", FirstName="A", LastName="B", Username= "test@example.com", Password = "Password123" };
            
            _userManagerMock.Setup(um => um.FindByEmailAsync(model.Email)).ReturnsAsync((NiyoUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<NiyoUser>(), model.Password)).ReturnsAsync(IdentityResult.Success)
            .Callback<NiyoUser, string>((user, pwd) =>
            {
                _context.Users.Add(user); // Add user to in-memory database
                _context.SaveChanges();
            });

            var signInManager = GetSignInManager();
            var userService = new UserService(_userManagerMock.Object, _configurationMock.Object, _loggerMock.Object, _context, signInManager, _mappingServiceMock.Object);

            var result = await userService.SignUpAsync(model);

            var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            Assert.NotNull(createdUser);
            Assert.Equal(model.Email, createdUser.Email);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<NiyoUser>(), model.Password), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var model = new SignInDTO { Email = "test@example.com", Password = "Password123" };
            var user = new NiyoUser { Email = model.Email, Id = "1", EmailConfirmed = true };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var signInManager = GetSignInManager();
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);

            _configurationMock.Setup(c => c.GetSection("JWTSettings")["Key"]).Returns("J2F7DB4F-4D62-7X37-YPRB-BB7L4UPHPWVS");
            _configurationMock.Setup(c => c.GetSection("JWTSettings")["Issuer"]).Returns("issuer");
            _configurationMock.Setup(c => c.GetSection("JWTSettings")["Audience"]).Returns("audience");

            var userService = new UserService(_userManagerMock.Object, _configurationMock.Object, _loggerMock.Object, _context, signInManager, _mappingServiceMock.Object);

            var result = await userService.SignInAsync(model);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            _userManagerMock.Verify(um => um.CheckPasswordAsync(user, model.Password), Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldMarkUserAsDeleted_WhenUserExists()
        {
            var user = new NiyoUser { Id = "1", Email = "test@example.com", IsDeleted = false };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var signInManager = GetSignInManager();
            var userService = new UserService(_userManagerMock.Object, _configurationMock.Object, _loggerMock.Object, _context, signInManager, _mappingServiceMock.Object);

            await userService.DeleteAccountAsync(user.Id);

            var deletedUser = await _context.Users.FindAsync(user.Id);
            Assert.True(deletedUser.IsDeleted);
        }

        [Fact]
        public async Task FetchAllUsers_ShouldReturnListOfUsers_WhenUsersExist()
        {
            var user1 = new NiyoUser { Id = "1", Email = "test1@example.com", IsDeleted = false };
            var user2 = new NiyoUser { Id = "2", Email = "test2@example.com", IsDeleted = false };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            _mappingServiceMock.Setup(ms => ms.MapUserToModel(It.IsAny<NiyoUser>())).Returns(new UserBindingDTO());

            var signInManager = GetSignInManager();
            var userService = new UserService(_userManagerMock.Object, _configurationMock.Object, _loggerMock.Object, _context, signInManager, _mappingServiceMock.Object);

            var result = await userService.FetchAllUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task FetchUserAsync_ShouldReturnUser_WhenUserExistsAndNotDeleted()
        {
            var user = new NiyoUser { Id = "1", Email = "test@example.com", IsDeleted = false };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mappingServiceMock.Setup(ms => ms.MapUserToModel(It.IsAny<NiyoUser>())).Returns(new UserBindingDTO());

            var signInManager = GetSignInManager();
            var userService = new UserService(_userManagerMock.Object, _configurationMock.Object, _loggerMock.Object, _context, signInManager, _mappingServiceMock.Object);

            var result = await userService.FetchUserAsync(user.Email);

            Assert.NotNull(result);
            _mappingServiceMock.Verify(ms => ms.MapUserToModel(It.IsAny<NiyoUser>()), Times.Once);
        }
    }
}
