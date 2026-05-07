using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using rpmtwo.Models;

namespace rpmtwo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AeroflotDbContext db;
        private User currentUser;
        private string userRole;
        public MainWindow(User user)
        {
            InitializeComponent();
            db = new AeroflotDbContext();
            currentUser = user;
            var role = db.Roles.FirstOrDefault(r => r.RoleId == currentUser.UserRole);
            userRole = role?.RoleName ?? "Unknown";
            LoadData();
            SetPermissionsByRole();
        }
        private void SetPermissionsByRole()
        {
            if (userRole != "Admin")
            {
                btnAdd.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
                // tbStatus.Text += " (Только просмотр)";
            }
        }
        private void LoadData()
        {
            try
            {
                var flights = db.Рейсы.ToList();
                dgFlights.ItemsSource = flights;
                tbStatus.Text = $"Ready. Total records: {flights.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Load error: {ex.Message}", "Error");
                tbStatus.Text = "Load error";
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var newFlight = new Рейсы();
            var window = new FlightWindow(newFlight, false);

            if (window.ShowDialog() == true)
            {
                try
                {
                    db.Рейсы.Add(newFlight);
                    db.SaveChanges();
                    LoadData();
                    tbStatus.Text = $"Added flight {newFlight.НомерРейса}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Add error: {ex.Message}", "Error");
                }
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedFlight = dgFlights.SelectedItem as Рейсы;

            if (selectedFlight == null)
            {
                MessageBox.Show("Please select a flight first!", "Warning");
                return;
            }

            var window = new FlightWindow(selectedFlight, false);

            if (window.ShowDialog() == true)
            {
                try
                {
                    db.Entry(selectedFlight).State = EntityState.Modified;
                    db.SaveChanges();
                    LoadData();
                    tbStatus.Text = $"Updated flight {selectedFlight.НомерРейса}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Edit error: {ex.Message}", "Error");
                }
            }
        }
        private void View_Click(object sender, RoutedEventArgs e)
        {
            var selectedFlight = dgFlights.SelectedItem as Рейсы;

            if (selectedFlight == null)
            {
                MessageBox.Show("Please select a flight first!", "Warning");
                return;
            }

            var window = new FlightWindow(selectedFlight, true);
            window.ShowDialog();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedFlight = dgFlights.SelectedItem as Рейсы;

            if (selectedFlight == null)
            {
                MessageBox.Show("Please select a flight to delete!", "Warning");
                return;
            }

            var result = MessageBox.Show($"Delete flight {selectedFlight.НомерРейса}?",
                          "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.Рейсы.Remove(selectedFlight);
                    db.SaveChanges();
                    LoadData();
                    tbStatus.Text = $"Deleted flight {selectedFlight.НомерРейса}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Delete error: {ex.Message}", "Error");
                }
            }
        }
        private void dgFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void QuerySelectByCity(object sender, RoutedEventArgs e)
        {
            try
            {
                var city = txtCity.Text;
                var results = db.Рейсы
                    .Where(f => f.ПунктНазначения.Contains(city))
                    .ToList();

                lbResults.Items.Clear();
                if (results.Count == 0)
                {
                    lbResults.Items.Add("No flights found");
                }
                else
                {
                    foreach (var f in results)
                    {
                        lbResults.Items.Add($"{f.НомерРейса} -> {f.ПунктНазначения} | Free seats: {f.СвободныхМест}");
                    }
                }
                tbStatus.Text = $"Found: {results.Count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query error: {ex.Message}", "Error");
            }
        }

        // Query 6a: Select by date
        private void QuerySelectByDate(object sender, RoutedEventArgs e)
        {
            try
            {
                var date = dpDate.SelectedDate ?? DateTime.Now;
                var results = db.Рейсы
                    .Where(f => f.ВремяВылета.Date >= date.Date)
                    .ToList();

                lbResults.Items.Clear();
                foreach (var f in results)
                {
                    lbResults.Items.Add($"{f.НомерРейса} | {f.ПунктНазначения} | {f.ВремяВылета:dd.MM.yyyy HH:mm}");
                }
                tbStatus.Text = $"Found: {results.Count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query error: {ex.Message}", "Error");
            }
        }

        // Query 6a: Select by available seats
        private void QuerySelectBySeats(object sender, RoutedEventArgs e)
        {
            try
            {
                int minSeats = int.Parse(txtMinSeats.Text);
                var results = db.Рейсы
                    .Where(f => f.СвободныхМест > minSeats)
                    .ToList();

                lbResults.Items.Clear();
                foreach (var f in results)
                {
                    lbResults.Items.Add($"{f.НомерРейса} | {f.ПунктНазначения} | Free: {f.СвободныхМест}/{f.Вместимость}");
                }
                tbStatus.Text = $"Found: {results.Count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query error: {ex.Message}", "Error");
            }
        }

        // Query 6b: Update - increase seats by percent
        private void QueryUpdateSeats(object sender, RoutedEventArgs e)
        {
            try
            {
                int percent = int.Parse(txtPercent.Text);
                var flights = db.Рейсы.ToList();

                foreach (var f in flights)
                {
                    int increase = (int)(f.СвободныхМест * percent / 100.0);
                    f.СвободныхМест = Math.Min(f.СвободныхМест + increase, f.Вместимость);
                }

                db.SaveChanges();
                LoadData();

                lbResults.Items.Clear();
                lbResults.Items.Add($"✓ Increased available seats by {percent}% for all flights");
                tbStatus.Text = "Update completed";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update error: {ex.Message}", "Error");
            }
        }

        // Query 6b: Update - change aircraft type
        private void QueryUpdateAircraft(object sender, RoutedEventArgs e)
        {
            try
            {
                string oldType = txtOldType.Text;
                string newType = txtNewType.Text;

                var flightsToUpdate = db.Рейсы.Where(f => f.ТипСамолета == oldType).ToList();
                int count = flightsToUpdate.Count;

                foreach (var f in flightsToUpdate)
                {
                    f.ТипСамолета = newType;
                }

                db.SaveChanges();
                LoadData();

                lbResults.Items.Clear();
                lbResults.Items.Add($"✓ Changed aircraft type: {oldType} → {newType}");
                lbResults.Items.Add($"Updated flights: {count}");
                tbStatus.Text = $"Updated {count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update error: {ex.Message}", "Error");
            }
        }

        // Query 6c: Delete by city
        private void QueryDeleteByCity(object sender, RoutedEventArgs e)
        {
            try
            {
                string city = txtDeleteCity.Text;
                var flightsToDelete = db.Рейсы.Where(f => f.ПунктНазначения == city).ToList();
                int count = flightsToDelete.Count;

                db.Рейсы.RemoveRange(flightsToDelete);
                db.SaveChanges();
                LoadData();

                lbResults.Items.Clear();
                lbResults.Items.Add($"✓ Deleted {count} flights to {city}");
                tbStatus.Text = $"Deleted {count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete error: {ex.Message}", "Error");
            }
        }

        // Query 6c: Delete flights with zero available seats
        private void QueryDeleteZeroSeats(object sender, RoutedEventArgs e)
        {
            try
            {
                var flightsToDelete = db.Рейсы.Where(f => f.СвободныхМест == 0).ToList();
                int count = flightsToDelete.Count;

                db.Рейсы.RemoveRange(flightsToDelete);
                db.SaveChanges();
                LoadData();

                lbResults.Items.Clear();
                lbResults.Items.Add($"✓ Deleted {count} flights with 0 available seats");
                tbStatus.Text = $"Deleted {count} flights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete error: {ex.Message}", "Error");
            }
        }

        // Clear results list
        private void ClearResults_Click(object sender, RoutedEventArgs e)
        {
            lbResults.Items.Clear();
            tbStatus.Text = "Results cleared";
        }

        // Menu handlers
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aeroflot Flight Management System\nVersion 1.0", "About");
        }
    }
}
    
