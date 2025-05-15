using DMSMicroservice.AuthService.DTOs;
using DMSMicroservice.AuthService.Models;
using DMSMicroservice.AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DMSMicroservice.AuthService.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost(ApiEndPoints.AuthEndPoints.Login)]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ResponseDto
                {
                    Email = request.Email,
                    Message = "User not found",
                    Error = "Invalid credentials",
                    Token = null!
                };

            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordCheck)
                return new ResponseDto
                {
                    Email = request.Email,
                    Message = "Invalid password",
                    Error = "Invalid credentials",
                    Token = null!
                };

            // Convert the list of roles to a single string (e.g., comma-separated)
            var roles = await _userManager.GetRolesAsync(user);
            user.Role = string.Join(", ", roles);

            return new ResponseDto
            {
                Email = user.Email!,
                Message = "Login successful",
                Error = null!,
                Token = await _tokenService.GenerateToken(user)
            };
        }

        [HttpPost(ApiEndPoints.AuthEndPoints.Register)]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var user = new ApplicationUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName  ,
                Address = registerRequest.Address,
                City = registerRequest.City,
                State = registerRequest.State,
                Country = registerRequest.Country,
                DateOfBirth = registerRequest.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed=true,
                PhoneNumberConfirmed = true,
                Role = registerRequest.Role

            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {

              var roleResult=  await _userManager.AddToRoleAsync(user, registerRequest.Role);
                if(!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new ResponseDto
                    {
                        Email = registerRequest.Email,
                        Message = "User creation failed",
                        Error = string.Join(", ", roleResult.Errors.Select(e => e.Description)),
                        Token = null!
                    });
                }
                return Ok(new ResponseDto
                {
                    Email = registerRequest.Email,
                    Message = "User created successfully",
                    Error = null!,
                    Token = await _tokenService.GenerateToken(user)
                }); 
            }
            else
            {               
                return BadRequest(new ResponseDto
                {
                    Email = registerRequest.Email,
                    Message = "User creation failed",
                    Error = string.Join(", ", result.Errors.Select(e => e.Description)),
                    Token = null!
                });
            }
        }
    }
}
