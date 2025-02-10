using System;

namespace Bookworms.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }  // Primary Key
        public string UserId { get; set; }  // FK to ApplicationUser
        public string HashedPassword { get; set; }  // Store the hashed password
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date of password change
    }
}
    