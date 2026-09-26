using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SecurityIncidentTracker.Data;
using SecurityIncidentTracker.Models;

namespace SecurityIncidentTracker.Views
{
    /// <summary>
    /// Interaction logic for IncidentWindow.xaml.
    /// Manages incident CRUD operations, real-time search, and status filtering.
    /// </summary>
    public partial class IncidentWindow : Window
    {
        private readonly User _currentUser;
        private Incident? _selectedIncident = null;

        public IncidentWindow(User? currentUser = null)
        {
            InitializeComponent();
            _currentUser = currentUser ?? new User { Username = "admin", Role = "Administrator" };
            lblUserBadge.Text = $"👤 {_currentUser.Username}";

            // Set default date to today
            dpDate.SelectedDate = DateTime.Today;

            // Load initial records
            LoadIncidents();
        }

        /// <summary>
        /// Retrieves incidents from SQLite matching search and filter criteria.
        /// </summary>
        private void LoadIncidents()
        {
            try
            {
                using var db = new AppDbContext();
                var query = db.Incidents.AsQueryable();

                int totalCount = query.Count();

                // 1. Text Search Filter (ID, Type, Description)
                string searchText = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(i =>
                        i.Id.ToString().Contains(searchText) ||
                        i.IncidentType.ToLower().Contains(searchText) ||
                        i.Description.ToLower().Contains(searchText));
                }

                // 2. Severity Filter
                string severityFilter = GetSelectedComboBoxText(cmbFilterSeverity);
                if (!string.IsNullOrEmpty(severityFilter) && severityFilter != "All Severities")
                {
                    query = query.Where(i => i.Severity == severityFilter);
                }

                // 3. Status Filter
                string statusFilter = GetSelectedComboBoxText(cmbFilterStatus);
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All Statuses")
                {
                    query = query.Where(i => i.Status == statusFilter);
                }

                var filteredList = query.OrderByDescending(i => i.Id).ToList();
                dgIncidents.ItemsSource = filteredList;

                lblRecordCount.Text = $"Showing {filteredList.Count} of {totalCount} Incidents";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load incidents: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Search and Filter Events

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadIncidents();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadIncidents();
            }
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only reload if the window has finished initializing
            if (IsLoaded)
            {
                LoadIncidents();
            }
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbFilterSeverity.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            LoadIncidents();
        }

        #endregion

        #region DataGrid Selection and Form Binding

        private void DgIncidents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgIncidents.SelectedItem is Incident selected)
            {
                _selectedIncident = selected;
                PopulateForm(selected);
            }
            else
            {
                _selectedIncident = null;
            }
        }

        private void PopulateForm(Incident incident)
        {
            txtIncidentId.Text = incident.Id.ToString();
            SetComboBoxItem(cmbIncidentType, incident.IncidentType);
            SetComboBoxItem(cmbSeverity, incident.Severity);
            SetComboBoxItem(cmbStatus, incident.Status);
            dpDate.SelectedDate = incident.Date;
            txtDescription.Text = incident.Description;

            // Update UI state for Edit mode
            lblFormTitle.Text = $"✏️ Edit Incident #{incident.Id}";
            lblFormSubtitle.Text = "Modify values and click Update or Delete";
            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }

        private void ResetForm()
        {
            _selectedIncident = null;
            dgIncidents.SelectedItem = null;

            txtIncidentId.Text = "Auto-generated on Add";
            cmbIncidentType.SelectedIndex = -1;
            cmbSeverity.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            dpDate.SelectedDate = DateTime.Today;
            txtDescription.Clear();

            // Reset UI state for Add mode
            lblFormTitle.Text = "📝 Add New Incident";
            lblFormSubtitle.Text = "Enter incident details and click Add";
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        #endregion

        #region CRUD Operations

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                using var db = new AppDbContext();

                var newIncident = new Incident
                {
                    IncidentType = GetSelectedComboBoxText(cmbIncidentType),
                    Severity = GetSelectedComboBoxText(cmbSeverity),
                    Status = GetSelectedComboBoxText(cmbStatus),
                    Date = dpDate.SelectedDate ?? DateTime.Now,
                    Description = txtDescription.Text.Trim()
                };

                db.Incidents.Add(newIncident);
                db.SaveChanges();

                MessageBox.Show($"Incident #{newIncident.Id} ({newIncident.IncidentType}) successfully logged!", 
                    "Incident Created", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetForm();
                LoadIncidents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save incident: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIncident == null)
            {
                MessageBox.Show("Please select an incident from the table to update.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateForm())
                return;

            try
            {
                using var db = new AppDbContext();
                var incident = db.Incidents.Find(_selectedIncident.Id);

                if (incident == null)
                {
                    MessageBox.Show("Incident not found in database. It may have been deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetForm();
                    LoadIncidents();
                    return;
                }

                incident.IncidentType = GetSelectedComboBoxText(cmbIncidentType);
                incident.Severity = GetSelectedComboBoxText(cmbSeverity);
                incident.Status = GetSelectedComboBoxText(cmbStatus);
                incident.Date = dpDate.SelectedDate ?? DateTime.Now;
                incident.Description = txtDescription.Text.Trim();

                db.SaveChanges();

                MessageBox.Show($"Incident #{incident.Id} successfully updated!", 
                    "Update Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetForm();
                LoadIncidents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update incident: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIncident == null)
            {
                MessageBox.Show("Please select an incident from the table to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to permanently delete Incident #{_selectedIncident.Id}?\n\nType: {_selectedIncident.IncidentType}\nDescription: {_selectedIncident.Description}",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var incident = db.Incidents.Find(_selectedIncident.Id);

                if (incident != null)
                {
                    db.Incidents.Remove(incident);
                    db.SaveChanges();

                    MessageBox.Show($"Incident #{_selectedIncident.Id} has been deleted.", 
                        "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ResetForm();
                LoadIncidents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete incident: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        #endregion

        #region Helper & Validation Methods

        private bool ValidateForm()
        {
            if (cmbIncidentType.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Incident Type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbIncidentType.Focus();
                return false;
            }

            if (cmbSeverity.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a Severity level.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSeverity.Focus();
                return false;
            }

            if (cmbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a Status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbStatus.Focus();
                return false;
            }

            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a valid Date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDate.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please provide a Description for the incident.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return false;
            }

            return true;
        }

        private string GetSelectedComboBoxText(ComboBox cmb)
        {
            if (cmb.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? string.Empty;
            }
            return cmb.Text ?? string.Empty;
        }

        private void SetComboBoxItem(ComboBox cmb, string text)
        {
            foreach (var item in cmb.Items)
            {
                if (item is ComboBoxItem cbi && string.Equals(cbi.Content?.ToString(), text, StringComparison.OrdinalIgnoreCase))
                {
                    cmb.SelectedItem = cbi;
                    return;
                }
            }
            cmb.Text = text;
        }

        #endregion

        #region Navigation Events

        private void BtnBackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboard = new DashboardWindow(_currentUser);
            dashboard.Show();
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

        #endregion
    }
}
