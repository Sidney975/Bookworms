using System;

namespace Bookworms.Models
{
    public class AuditLog
    {
        public int Id { get; set; }  // Primary Key
        public string UserId { get; set; }  // ID of the user performing the action
        public string Action { get; set; }  // What action they performed
        public string Description { get; set; }  // Additional details
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // When it happened
    }
}
