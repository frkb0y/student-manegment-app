using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System;
using plz_fix.Models;

namespace plz_fix.pages.dash
{
    public partial class task : ContentPage
    {
        public task()
        {
            InitializeComponent();
            LoadTasksFromDatabase();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            LoadTasksFromDatabase();
        }

        private async void LoadTasksFromDatabase()
        {
            string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            var tasks = new List<TodoModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT OID, WorkName, WorkDescription, StartDate FROM Todo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tasks.Add(new TodoModel
                            {
                                OID = Convert.ToInt32(reader["OID"]),
                                WorkName = reader["WorkName"].ToString(),
                                WorkDescription = reader["WorkDescription"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"])
                            });
                        }
                    }
                }

                // Bind the tasks to the CollectionView
                TaskListView.ItemsSource = tasks;

                // If no tasks are found, display empty view
                if (tasks.Count == 0)
                {
                    await DisplayAlert("No Tasks", "You have no tasks at the moment.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            }
        }

        private async void OnTaskTapped(object sender, EventArgs e)
        {
            var task = (sender as Frame)?.BindingContext as TodoModel;
            if (task != null)
                await Navigation.PushAsync(new TaskEditPage(task));
        }

        private async void OnCalendarButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalendarPage());
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskEditPage(null));
        }
    }
}
