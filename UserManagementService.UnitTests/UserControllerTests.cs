using DMSMicroservice.UserManagementService.Controllers;
using DMSMicroservice.UserManagementService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UserManagementService.UnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserManager = MockUserManager();
            _mockRoleManager = MockRoleManager();
            _controller = new UserController(_mockUserManager.Object, _mockRoleManager.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WithUsersAndRoles()
        {
            // Arrange
            var users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", Email = "user1@example.com", PhoneNumber = "1234567890" },
                new IdentityUser { Id = "2", Email = "user2@example.com", PhoneNumber = "0987654321" }
            };
            _mockUserManager.Setup(m => m.Users).Returns(users.AsQueryable());
            _mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _controller.DeleteUser("nonexistent-id");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest_WhenUserIsAdmin()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", Email = "admin@example.com" };
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", Email = "user@example.com" };
            _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
            _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser("1");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllRoles_ReturnsOkResult_WithRoles()
        {
            // Arrange
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "1", Name = "Admin" },
                new IdentityRole { Id = "2", Name = "User" }
            };
            _mockRoleManager.Setup(m => m.Roles).Returns(roles.AsQueryable());

            // Act
            var result = await _controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRoles = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, returnedRoles.Count);
        }

        [Fact]
        public async Task AddNewRole_ReturnsBadRequest_WhenRoleAlreadyExists()
        {
            // Arrange
            _mockRoleManager.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddNewRole("Admin");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddNewRole_ReturnsOk_WhenRoleIsAddedSuccessfully()
        {
            // Arrange
            _mockRoleManager.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockRoleManager.Setup(m => m.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.AddNewRole("NewRole");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        // Additional tests for other methods...

        private Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }
    }
}
