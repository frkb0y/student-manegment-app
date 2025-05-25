using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageNotesPage : ContentPage
    {
        public ManageNotesPage()
        {
            InitializeComponent();
            LoadStudents();
            LoadSubjects();
        }

        private async void LoadStudents()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Query to fetch students (filtered by role)
                    string query = @"
                        SELECT p.OID, p.FirstName, p.LastName 
                        FROM Profile p
                        INNER JOIN UserApp u ON u.Profile = p.OID
                        INNER JOIN AppRole r ON u.Role = r.OID
                        WHERE r.RoleName = 'Student' AND p.GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string name = $"{reader.GetString(1)} {reader.GetString(2)}";
                                StudentPicker.Items.Add($"{id} - {name}");
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

        private async void LoadSubjects()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Query to fetch subjects
                    string query = "SELECT OID, Name FROM Subject WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string subjectName = reader.GetString(1);
                                SubjectPicker.Items.Add($"{id} - {subjectName}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load subjects: {ex.Message}", "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (StudentPicker.SelectedIndex == -1 || SubjectPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Validation Error", "Please select a student and a subject.", "OK");
                return;
            }

            string selectedStudent = StudentPicker.SelectedItem.ToString();
            string selectedSubject = SubjectPicker.SelectedItem.ToString();

            int studentId = int.Parse(selectedStudent.Split('-')[0].Trim());
            int subjectId = int.Parse(selectedSubject.Split('-')[0].Trim());

            string comment = CommentEditor.Text;
            int score;
            string noteType;

            if (!int.TryParse(ScoreEntry.Text, out score))
            {
                await DisplayAlert("Validation Error", "Please enter a valid numeric score.", "OK");
                return;
            }

            // Determine note type based on score
            if (score < 10)
            {
                noteType = "Bad";
            }
            else if (score <= 14)
            {
                noteType = "Bad";
            }
            else
            {
                noteType = "Good";
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // Insert query for the new note
                    string query = @"
                        INSERT INTO Note (Score, Comments, Date, NoteType, Subject, Student)
                        VALUES (@score, @comment, GETDATE(), @noteType, @subjectId, @studentId)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Parameters.AddWithValue("@comment", string.IsNullOrWhiteSpace(comment) ? DBNull.Value : comment);
                        cmd.Parameters.AddWithValue("@noteType", noteType);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        cmd.Parameters.AddWithValue("@studentId", studentId);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await DisplayAlert("Success", "Note saved successfully!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save note: {ex.Message}", "OK");
            }
        }
    }
}