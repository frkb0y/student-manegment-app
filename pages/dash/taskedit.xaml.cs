using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using plz_fix.Models;
using System;

namespace plz_fix.pages.dash
{
    public partial class TaskEditPage : ContentPage
    {
        private TodoModel currentTask;

        public TaskEditPage(TodoModel task)
        {
            InitializeComponent();
            currentTask = task;

            if (currentTask != null)
            {
                WorkNameEntry.Text = task.WorkName;
                DescriptionEditor.Text = task.WorkDescription;
                StartDatePicker.Date = task.StartDate;
                StartTimePicker.Time = task.StartDate.TimeOfDay;
                CompletedCheckBox.IsChecked = task.IsCompleted;
            }
            else
            {
                StartDatePicker.Date = DateTime.Today;
                StartTimePicker.Time = DateTime.Now.TimeOfDay;
                CompletedCheckBox.IsChecked = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            DateTime combinedDate = StartDatePicker.Date + StartTimePicker.Time;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query;

                    if (currentTask == null)
                    {
                        query = "INSERT INTO Todo (WorkName, WorkDescription, StartDate, IsCompleted) VALUES (@name, @desc, @start, @isCompleted)";
                    }
                    else
                    {
                        query = "UPDATE Todo SET WorkName = @name, WorkDescription = @desc, StartDate = @start, IsCompleted = @isCompleted WHERE OID = @oid";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", WorkNameEntry.Text);
                        cmd.Parameters.AddWithValue("@desc", DescriptionEditor.Text);
                        cmd.Parameters.AddWithValue("@start", combinedDate);
                        cmd.Parameters.AddWithValue("@isCompleted", CompletedCheckBox.IsChecked);

                        if (currentTask != null)
                            cmd.Parameters.AddWithValue("@oid", currentTask.OID);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await DisplayAlert("Success", "Task saved successfully!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save task: {ex.Message}", "OK");
            }
        }
    }
}
