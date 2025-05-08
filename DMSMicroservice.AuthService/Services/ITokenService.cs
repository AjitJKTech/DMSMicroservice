using DMSMicroservice.AuthService.Models;

namespace DMSMicroservice.AuthService.Services
{
    public interface ITokenService
    {
       Task<string> GenerateToken(ApplicationUser user);
    }
}
