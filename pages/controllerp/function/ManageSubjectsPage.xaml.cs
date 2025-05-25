using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageSubjectsPage : ContentPage
    {
        private ObservableCollection<Subject> SubjectsList = new ObservableCollection<Subject>();

        public ManageSubjectsPage()
        {
            InitializeComponent();
            LoadSubjects();
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

                    string query = "SELECT OID, Name FROM Subject WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                SubjectsList.Add(new Subject
                                {
                                    OID = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                SubjectsCollectionView.ItemsSource = SubjectsList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load subjects: {ex.Message}", "OK");
            }
        }

        private async void OnAddSubjectClicked(object sender, EventArgs e)
        {
            string newSubjectName = NewSubjectEntry.Text;

            if (string.IsNullOrWhiteSpace(newSubjectName))
            {
                await DisplayAlert("Validation Error", "Subject name cannot be empty.", "OK");
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO Subject (Name) VALUES (@name)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", newSubjectName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await DisplayAlert("Success", "Subject added successfully!", "OK");
                NewSubjectEntry.Text = string.Empty;

                // Refresh the subjects list
                LoadSubjects();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add subject: {ex.Message}", "OK");
            }
        }
    }

    public class Subject
    {
        public int OID { get; set; }
        public string Name { get; set; }
    }
}