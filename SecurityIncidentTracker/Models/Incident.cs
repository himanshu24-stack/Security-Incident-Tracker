using System;

namespace SecurityIncidentTracker.Models
{
    /// <summary>
    /// Represents a security incident record tracked by the application.
    /// </summary>
    public class Incident
    {
        public int Id { get; set; }
        public string IncidentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "Medium";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Open";
    }
}
