using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SecurityIncidentTracker.Models;

namespace SecurityIncidentTracker.Data
{
    /// <summary>
    /// Application Database Context powered by Entity Framework Core and SQLite.
    /// Manages database tables and automatic database initialization/seeding.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Incident> Incidents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Store database file in the application running directory
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "security_tracker.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        /// <summary>
        /// Automatically creates the SQLite database and seeds default admin and sample incidents.
        /// Called during application startup.
        /// </summary>
        public static void InitializeDatabase()
        {
            using var db = new AppDbContext();
            
            // Create database file and tables if they don't already exist
            db.Database.EnsureCreated();

            // Seed default administrator account if no users exist
            if (!db.Users.Any())
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    Password = "admin123",
                    Role = "Administrator"
                });
                db.SaveChanges();
            }

            // Seed sample incidents for immediate demonstration and viva presentation
            if (!db.Incidents.Any())
            {
                db.Incidents.AddRange(
                    new Incident
                    {
                        IncidentType = "Phishing",
                        Description = "Suspicious email with fake invoice attachment reported by Finance department.",
                        Severity = "High",
                        Date = DateTime.Now.AddDays(-6),
                        Status = "Resolved"
                    },
                    new Incident
                    {
                        IncidentType = "Brute Force",
                        Description = "Multiple repeated failed SSH login attempts detected against main server from external IP.",
                        Severity = "High",
                        Date = DateTime.Now.AddDays(-4),
                        Status = "Open"
                    },
                    new Incident
                    {
                        IncidentType = "Malware",
                        Description = "Trojan signature detected on Lab PC-08 during scheduled endpoint antivirus scan.",
                        Severity = "Critical",
                        Date = DateTime.Now.AddDays(-2),
                        Status = "Investigating"
                    },
                    new Incident
                    {
                        IncidentType = "Unauthorized Access",
                        Description = "Unregistered USB flash drive inserted into restricted server room terminal.",
                        Severity = "Medium",
                        Date = DateTime.Now.AddDays(-1),
                        Status = "Open"
                    },
                    new Incident
                    {
                        IncidentType = "Suspicious Activity",
                        Description = "Abnormal outbound traffic spike on port 4444 after office hours.",
                        Severity = "Low",
                        Date = DateTime.Now,
                        Status = "Investigating"
                    }
                );
                db.SaveChanges();
            }
        }
    }
}
