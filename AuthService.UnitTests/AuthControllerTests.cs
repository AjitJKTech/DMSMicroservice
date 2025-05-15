using DMSMicroservice.AuthService.Controllers;
using DMSMicroservice.AuthService.DTOs;
using DMSMicroservice.AuthService.Models;
using DMSMicroservice.AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.UnitTests
{
    public class AuthControllerTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _userManagerMock = MockUserManager();
            _controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object);
        }

        // Helper to mock UserManager
        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        #region Login Tests

        [Fact]
        public async Task Login_UserNotFound_ReturnsUserNotFoundResponse()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "password" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var response = Assert.IsType<ResponseDto>(result.Value);
            Assert.Equal("User not found", response.Message);
            Assert.Equal("Invalid credentials", response.Error);
            Assert.Null(response.Token);
            Assert.Equal(request.Email, response.Email);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsInvalidPasswordResponse()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" };
            var request = new LoginRequest { Email = "test@example.com", Password = "wrong" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var response = Assert.IsType<ResponseDto>(result.Value);
            Assert.Equal("Invalid password", response.Message);
            Assert.Equal("Invalid credentials", response.Error);
            Assert.Null(response.Token);
            Assert.Equal(request.Email, response.Email);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" };
            var request = new LoginRequest { Email = "test@example.com", Password = "correct" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _tokenServiceMock.Setup(x => x.GenerateToken(user)).ReturnsAsync("token123");

            // Act
            var result = await _controller.Login(request);

            // Assert
            var response = Assert.IsType<ResponseDto>(result.Value);
            Assert.Equal("Login successful", response.Message);
            Assert.Null(response.Error);
            Assert.Equal("token123", response.Token);
            Assert.Equal(request.Email, response.Email);
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_Successful_ReturnsOkWithToken()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "newuser@example.com",
                Password = "Password1!",
                Role = "User",
                FirstName = "First",
                LastName = "Last",
                PhoneNumber = "1234567890",
                Address = "123 St",
                City = "City",
                State = "State",
                Country = "Country",
                DateOfBirth = DateTime.UtcNow
            };
            var user = new ApplicationUser { Email = registerRequest.Email, Role = registerRequest.Role };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerRequest.Role))
                .ReturnsAsync(IdentityResult.Success);
            _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("token123");

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(okResult.Value);
            Assert.Equal("User created successfully", response.Message);
            Assert.Null(response.Error);
            Assert.Equal(registerRequest.Email, response.Email);
            Assert.Equal("token123", response.Token);
        }

        [Fact]
        public async Task Register_AddToRoleFails_DeletesUserAndReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "failrole@example.com",
                Password = "Password1!",
                Role = "User",
                FirstName = "First",
                LastName = "Last",
                PhoneNumber = "1234567890",
                Address = "123 St",
                City = "City",
                State = "State",
                Country = "Country",
                DateOfBirth = DateTime.UtcNow
            };
            var user = new ApplicationUser { Email = registerRequest.Email, Role = registerRequest.Role };
            var roleError = new IdentityError { Description = "Role error" };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), registerRequest.Role))
                .ReturnsAsync(IdentityResult.Failed(roleError));
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequest.Value);
            Assert.Equal("User creation failed", response.Message);
            Assert.Contains("Role error", response.Error);
            Assert.Equal(registerRequest.Email, response.Email);
            Assert.Null(response.Token);
        }

        [Fact]
        public async Task Register_CreateUserFails_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Email = "failcreate@example.com",
                Password = "Password1!",
                Role = "User",
                FirstName = "First",
                LastName = "Last",
                PhoneNumber = "1234567890",
                Address = "123 St",
                City = "City",
                State = "State",
                Country = "Country",
                DateOfBirth = DateTime.UtcNow
            };
            var createError = new IdentityError { Description = "Create error" };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Failed(createError));

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequest.Value);
            Assert.Equal("User creation failed", response.Message);
            Assert.Contains("Create error", response.Error);
            Assert.Equal(registerRequest.Email, response.Email);
            Assert.Null(response.Token);
        }

        #endregion
    }
}
