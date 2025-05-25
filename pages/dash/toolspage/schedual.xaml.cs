using Microsoft.Maui.Controls;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Graphics;

namespace plz_fix.pages.dash.toolspage
{
    public partial class schedual : ContentPage
    {
        private string _username;
        public ObservableCollection<SchedulerAppointment> Appointments { get; set; } = new();

        public schedual(string username)
        {
            InitializeComponent();
            _username = username;
            BindingContext = this;
            LoadSchedulerAppointments(_username);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void LoadSchedulerAppointments(string username)
        {
            string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";

            try
            {
                using SqlConnection conn = new(connectionString);
                await conn.OpenAsync();

                // Get user OID
                string getUserOidQuery = "SELECT OID FROM UserApp WHERE Username = @Username";
                int userOid = -1;

                using (SqlCommand cmd = new(getUserOidQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                        userOid = Convert.ToInt32(result);
                    else
                    {
                        await DisplayAlert("Error", "User not found", "OK");
                        return;
                    }
                }

                // Get user tasks
                string getTasksQuery = @"
                    SELECT WorkName, WorkDescription, StartDate
                    FROM Todo
                    WHERE AssignedTo = @UserOid";

                using SqlCommand taskCmd = new(getTasksQuery, conn);
                taskCmd.Parameters.AddWithValue("@UserOid", userOid);

                using var reader = await taskCmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Appointments.Add(new SchedulerAppointment
                    {
                        Subject = reader["WorkName"].ToString(),
                        Location = reader["WorkDescription"].ToString(),
                        StartTime = Convert.ToDateTime(reader["StartDate"]),
                        EndTime = Convert.ToDateTime(reader["StartDate"]).AddHours(1),
                        Background = Colors.MediumPurple
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load schedule: {ex.Message}", "OK");
            }
        }
    }
}
