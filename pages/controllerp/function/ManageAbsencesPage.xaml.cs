using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageAbsencesPage : ContentPage
    {
        public ObservableCollection<AbsenceStudent> StudentsList { get; set; } = new ObservableCollection<AbsenceStudent>();
        public ObservableCollection<string> ClassesList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SallesList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SubjectsList { get; set; } = new ObservableCollection<string>();

        public ManageAbsencesPage()
        {
            InitializeComponent();
            LoadStudents();
            LoadClasses();
            LoadSalles();
            LoadSubjects();

            StudentsCollectionView.ItemsSource = StudentsList;
            ClassPicker.ItemsSource = ClassesList;
            SallePicker.ItemsSource = SallesList;
            SubjectPicker.ItemsSource = SubjectsList;
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
                                StudentsList.Add(new AbsenceStudent
                                {
                                    OID = Convert.ToInt32(reader.GetValue(0)),
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

        private async void LoadClasses()
        {
            try
            {
                ClassesList.Clear();

                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT Oid, Name
                        FROM Classe
                        WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ClassesList.Add(reader.GetString(1)); // Name column
                            }
                        }
                    }
                }

                ClassPicker.ItemsSource = ClassesList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load classes: {ex.Message}", "OK");
            }
        }

        private async void LoadSalles()
        {
            try
            {
                SallesList.Clear();

                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT Name FROM Salle WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                SallesList.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                SallePicker.ItemsSource = SallesList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load salles: {ex.Message}", "OK");
            }
        }

        private async void LoadSubjects()
        {
            try
            {
                SubjectsList.Clear();

                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT Name FROM Subject WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                SubjectsList.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                SubjectPicker.ItemsSource = SubjectsList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load subjects: {ex.Message}", "OK");
            }
        }

        private async void OnRecordAbsenceClicked(object sender, EventArgs e)
        {
            var selectedStudents = StudentsList.Where(s => s.IsSelected).ToList();
            var selectedClass = ClassPicker.SelectedItem?.ToString();
            var selectedSalle = SallePicker.SelectedItem?.ToString();
            var selectedSubject = SubjectPicker.SelectedItem?.ToString();

            if (selectedStudents.Count == 0)
            {
                await DisplayAlert("Validation Error", "Please select at least one student.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(selectedClass) || string.IsNullOrEmpty(selectedSalle) || string.IsNullOrEmpty(selectedSubject))
            {
                await DisplayAlert("Validation Error", "Please select a class, salle, and subject.", "OK");
                return;
            }

            string absenceDetails = AbsenceDetailsEditor.Text;
            DateTime absenceDate = AbsenceDatePicker.Date;

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    foreach (var student in selectedStudents)
                    {
                        string query = @"
                            INSERT INTO AppointmentAttendance (Content, Class, Salle, StartDate, EndDate, Student, Subject, AttendanceStatus) 
                            VALUES (@content, @class, @salle, @startDate, @endDate, @student, @subject, @attendanceStatus)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@content", string.IsNullOrWhiteSpace(absenceDetails) ? DBNull.Value : absenceDetails);
                            cmd.Parameters.AddWithValue("@class", selectedClass);
                            cmd.Parameters.AddWithValue("@salle", selectedSalle);
                            cmd.Parameters.AddWithValue("@startDate", absenceDate);
                            cmd.Parameters.AddWithValue("@endDate", absenceDate.AddHours(1));
                            cmd.Parameters.AddWithValue("@student", student.OID);
                            cmd.Parameters.AddWithValue("@subject", selectedSubject);
                            cmd.Parameters.AddWithValue("@attendanceStatus", 0);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await DisplayAlert("Success", "Absence(s) recorded successfully!", "OK");

                AbsenceDetailsEditor.Text = string.Empty;
                AbsenceDatePicker.Date = DateTime.Now;
                foreach (var student in StudentsList)
                {
                    student.IsSelected = false;
                }
                ClassPicker.SelectedIndex = -1;
                SallePicker.SelectedIndex = -1;
                SubjectPicker.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to record absence: {ex.Message}", "OK");
            }
        }
    }

    public class AbsenceStudent
    {
        public int OID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}