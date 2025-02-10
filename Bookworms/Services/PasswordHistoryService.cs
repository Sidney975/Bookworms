using Bookworms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bookworms.Services
{
    public class PasswordHistoryService
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordHistoryService(AuthDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsPasswordReusedAsync(ApplicationUser user, string newPassword)
        {
            var passwordHistories = await _context.PasswordHistories
                .Where(ph => ph.UserId == user.Id)
                .OrderByDescending(ph => ph.CreatedAt)
                .Take(5)  
                .ToListAsync();

            foreach (var oldPassword in passwordHistories)
            {
                var fakeUser = new ApplicationUser { Id = user.Id, PasswordHash = oldPassword.HashedPassword };
                bool isMatch = await _userManager.CheckPasswordAsync(fakeUser, newPassword);

                if (isMatch)
                {
                    return true; 
                }
            }
            return false;  
        }

        public async Task StorePasswordAsync(ApplicationUser user)
        {
            var passwordHash = user.PasswordHash;
            var passwordHistory = new PasswordHistory
            {
                UserId = user.Id,
                HashedPassword = passwordHash
            };

            _context.PasswordHistories.Add(passwordHistory);
            await _context.SaveChangesAsync();
        }
    }
}
