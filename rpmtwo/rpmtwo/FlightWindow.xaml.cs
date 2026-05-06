using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore.Metadata;
using rpmtwo.Models;

namespace rpmtwo
{
    /// <summary>
    /// Логика взаимодействия для FlightWindow.xaml
    /// </summary>
    public partial class FlightWindow : Window
    {
        public Рейсы CurrentFlight { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable => !IsReadOnly;

        public FlightWindow(Рейсы flight, bool readOnlyMode)
        {
            InitializeComponent();
            DataContext = this;

            CurrentFlight = flight;
            IsReadOnly = readOnlyMode;

            if (IsReadOnly)
            {
                Title = "View Flight";
                MakeReadOnly();
            }
            else if (flight.Id == 0)
            {
                Title = "Add Flight";
                if (CurrentFlight.ВремяВылета.Year == 1)
                {
                    CurrentFlight.ВремяВылета = DateTime.Now;
                    CurrentFlight.ВремяПрибытия = DateTime.Now.AddHours(2);
                }
            }
            else
            {
                Title = "Edit Flight";
            }

            LoadFlightData();
        }

        private void MakeReadOnly()
        {
            txtFlightNumber.IsReadOnly = true;
            txtDestination.IsReadOnly = true;
            txtDepartureTime.IsReadOnly = true;
            txtArrivalTime.IsReadOnly = true;
            txtAvailableSeats.IsReadOnly = true;
            txtAircraftType.IsReadOnly = true;
            txtCapacity.IsReadOnly = true;
            dpDepartureDate.IsEnabled = false;
            dpArrivalDate.IsEnabled = false;
            btnOk.Visibility = Visibility.Collapsed;
            btnCancel.Content = "Close";
        }

        private void LoadFlightData()
        {
            txtFlightNumber.Text = CurrentFlight.НомерРейса;
            txtDestination.Text = CurrentFlight.ПунктНазначения;
            dpDepartureDate.SelectedDate = CurrentFlight.ВремяВылета.Date;
            txtDepartureTime.Text = CurrentFlight.ВремяВылета.ToString("HH:mm");
            dpArrivalDate.SelectedDate = CurrentFlight.ВремяПрибытия.Date;
            txtArrivalTime.Text = CurrentFlight.ВремяПрибытия.ToString("HH:mm");
            txtAvailableSeats.Text = CurrentFlight.СвободныхМест.ToString();
            txtAircraftType.Text = CurrentFlight.ТипСамолета;
            txtCapacity.Text = CurrentFlight.Вместимость.ToString();
        }

        private void SaveFlightData()
        {
            CurrentFlight.НомерРейса = txtFlightNumber.Text;
            CurrentFlight.ПунктНазначения = txtDestination.Text;

            var depDate = dpDepartureDate.SelectedDate ?? DateTime.Now;
            var depTime = TimeSpan.Parse(txtDepartureTime.Text);
            CurrentFlight.ВремяВылета = depDate + depTime;

            var arrDate = dpArrivalDate.SelectedDate ?? DateTime.Now;
            var arrTime = TimeSpan.Parse(txtArrivalTime.Text);
            CurrentFlight.ВремяПрибытия = arrDate + arrTime;

            CurrentFlight.СвободныхМест = int.Parse(txtAvailableSeats.Text);
            CurrentFlight.ТипСамолета = txtAircraftType.Text;
            CurrentFlight.Вместимость = int.Parse(txtCapacity.Text);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFlightData();

                // Check if arrival time is after departure time
                if (CurrentFlight.ВремяПрибытия <= CurrentFlight.ВремяВылета)
                {
                    MessageBox.Show("Arrival time must be after departure time!", "Validation Error");
                    return;
                }

                // Check if available seats not exceed capacity
                if (CurrentFlight.СвободныхМест > CurrentFlight.Вместимость)
                {
                    MessageBox.Show("Available seats cannot exceed capacity!", "Validation Error");
                    return;
                }

                // Check if available seats not negative
                if (CurrentFlight.СвободныхМест < 0)
                {
                    MessageBox.Show("Available seats cannot be negative!", "Validation Error");
                    return;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
