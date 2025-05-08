using DMSMicroservice.UserManagementService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMSMicroservice.UserManagementService.Data
{
    public class UserManagementDbContext    : IdentityDbContext<IdentityUser>
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
        {
        }
       
    }
   
}
