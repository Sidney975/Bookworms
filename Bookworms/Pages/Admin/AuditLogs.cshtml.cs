using Bookworms.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookworms.Pages.Admin
{
    public class AuditLogsModel : PageModel
    {
        private readonly AuthDbContext _context;

        public AuditLogsModel(AuthDbContext context)
        {
            _context = context;
        }

        public List<AuditLog> Logs { get; set; }

        public async Task OnGetAsync()
        {
            Logs = await _context.AuditLogs.ToListAsync();
        }
    }
}
