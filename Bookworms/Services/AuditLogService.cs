using Bookworms.Models;
using System.Threading.Tasks;

namespace Bookworms.Services
{
    public class AuditLogService
    {
        private readonly AuthDbContext _context;

        public AuditLogService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string action, string description)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
