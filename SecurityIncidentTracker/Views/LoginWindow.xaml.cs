using System;
using System.Linq;
using System.Windows;
using SecurityIncidentTracker.Data;
using SecurityIncidentTracker.Models;

namespace SecurityIncidentTracker.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml.
    /// Handles user authentication against the SQLite database.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Ensure database and initial tables exist
            try
            {
                AppDbContext.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            // 1. Basic validation
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter your username.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter your password.");
                txtPassword.Focus();
                return;
            }

            // 2. Database authentication
            try
            {
                using var db = new AppDbContext();
                var user = db.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower() && u.Password == password);

                if (user != null)
                {
                    // Login successful -> navigate to Dashboard
                    lblError.Visibility = Visibility.Collapsed;
                    DashboardWindow dashboard = new DashboardWindow(user);
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Invalid username or password. Please try again.");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
