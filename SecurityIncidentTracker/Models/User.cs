namespace SecurityIncidentTracker.Models
{
    /// <summary>
    /// Represents a system user for authentication.
    /// In this minor project, a default administrator is seeded.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Administrator";
    }
}
