using System.Windows;
using SecurityIncidentTracker.Data;

namespace SecurityIncidentTracker
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// Ensures database initialization and seeding before launching the Login window.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Automatically create SQLite database and seed default admin + sample data
            AppDbContext.InitializeDatabase();
        }
    }
}
