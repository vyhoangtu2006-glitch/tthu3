namespace tthu3.Models
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; } // For demo only. Do NOT store plain text passwords in production.
        public string Role { get; set; } // "Admin" or "Tenant"
        public string DisplayName { get; set; }
        public string TenantId { get; set; } // optional link to tenant (if needed)
    }
}
