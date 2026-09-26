using System;
using System.Linq;
using System.Windows;
using SecurityIncidentTracker.Data;
using SecurityIncidentTracker.Models;

namespace SecurityIncidentTracker.Views
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml.
    /// Displays key incident metrics and summary counts from SQLite.
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;

        public DashboardWindow(User? currentUser = null)
        {
            InitializeComponent();
            _currentUser = currentUser ?? new User { Username = "admin", Role = "Administrator" };
            lblCurrentUser.Text = $"{_currentUser.Username} ({_currentUser.Role})";

            LoadStatistics();
        }

        /// <summary>
        /// Queries the SQLite database and updates summary counters and the recent incidents table.
        /// </summary>
        public void LoadStatistics()
        {
            try
            {
                using var db = new AppDbContext();

                // 1. Total incidents count
                int total = db.Incidents.Count();

                // 2. Open incidents count
                int open = db.Incidents.Count(i => i.Status == "Open");

                // 3. Investigating count
                int investigating = db.Incidents.Count(i => i.Status == "Investigating");

                // 4. Resolved count
                int resolved = db.Incidents.Count(i => i.Status == "Resolved");

                // 5. High or Critical incidents count
                int critical = db.Incidents.Count(i => i.Severity == "High" || i.Severity == "Critical");

                // Update UI text blocks
                lblTotalIncidents.Text = total.ToString();
                lblOpenIncidents.Text = open.ToString();
                lblInvestigatingIncidents.Text = investigating.ToString();
                lblResolvedIncidents.Text = resolved.ToString();
                lblCriticalIncidents.Text = critical.ToString();

                // Load top 5 recent incidents for quick overview
                var recentIncidents = db.Incidents
                    .OrderByDescending(i => i.Date)
                    .Take(5)
                    .ToList();

                dgRecentIncidents.ItemsSource = recentIncidents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load dashboard statistics: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void BtnNavIncidents_Click(object sender, RoutedEventArgs e)
        {
            IncidentWindow incidentWindow = new IncidentWindow(_currentUser);
            incidentWindow.Show();
            this.Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}
