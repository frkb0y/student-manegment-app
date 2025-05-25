using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageEventsPage : ContentPage
    {
        public ObservableCollection<Student> StudentsList { get; set; } = new ObservableCollection<Student>();

        // This will be the teacher's OID (user `t` with `OID = 7`)
        private const int TeacherOID = 7;

        public ManageEventsPage()
        {
            InitializeComponent();
            LoadStudents();
            StudentsCollectionView.ItemsSource = StudentsList;
        }

        private async void LoadStudents()
        {
            try
            {
                StudentsList.Clear();

                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Query to fetch valid students
                    string query = @"
                        SELECT u.OID, p.FirstName, p.LastName 
                        FROM UserApp u
                        INNER JOIN Profile p ON u.Profile = p.OID
                        WHERE u.Role = 1 AND u.GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                StudentsList.Add(new Student
                                {
                                    OID = reader.GetInt32(0),
                                    Name = $"{reader.GetString(1)} {reader.GetString(2)}",
                                    IsSelected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load students: {ex.Message}", "OK");
            }
        }

        private async void OnAssignEventClicked(object sender, EventArgs e)
        {
            var selectedStudents = StudentsList.Where(s => s.IsSelected).ToList();

            if (selectedStudents.Count == 0)
            {
                await DisplayAlert("Validation Error", "Please select at least one student.", "OK");
                return;
            }

            string eventName = EventNameEntry.Text;
            string eventDescription = EventDescriptionEditor.Text;
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date;

            if (string.IsNullOrWhiteSpace(eventName))
            {
                await DisplayAlert("Validation Error", "Event name cannot be empty.", "OK");
                return;
            }

            if (endDate < startDate)
            {
                await DisplayAlert("Validation Error", "End date cannot be earlier than start date.", "OK");
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var student in selectedStudents)
                    {
                        string query = @"
                            INSERT INTO Todo (WorkName, WorkDescription, StartDate, EndDate, CreatedDate, CreatedBy, AssignedTo, IsCompleted, Status) 
                            VALUES (@workName, @workDescription, @startDate, @endDate, GETDATE(), @createdBy, @assignedTo, 0, 'Pending')";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@workName", eventName);
                            cmd.Parameters.AddWithValue("@workDescription", string.IsNullOrWhiteSpace(eventDescription) ? DBNull.Value : eventDescription);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);
                            cmd.Parameters.AddWithValue("@createdBy", TeacherOID); // Teacher's OID
                            cmd.Parameters.AddWithValue("@assignedTo", student.OID); // Student's OID

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await DisplayAlert("Success", "Event assigned successfully!", "OK");

                // Clear inputs and selections
                EventNameEntry.Text = string.Empty;
                EventDescriptionEditor.Text = string.Empty;
                StartDatePicker.Date = DateTime.Now;
                EndDatePicker.Date = DateTime.Now;
                foreach (var student in StudentsList)
                {
                    student.IsSelected = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to assign event: {ex.Message}", "OK");
            }
        }
    }

    public class Student
    {
        public int OID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}