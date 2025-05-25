using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageToDoPage : ContentPage
    {
        public ObservableCollection<ClassItem> ClassesList { get; set; } = new ObservableCollection<ClassItem>();
        public ObservableCollection<Student2> StudentsList { get; set; } = new ObservableCollection<Student2>();

        // This will be the teacher's OID (change if needed)
        private const int TeacherOID = 7;

        public ManageToDoPage()
        {
            InitializeComponent();
            LoadClasses();
            ClassPicker.ItemsSource = ClassesList;
            StudentsCollectionView.ItemsSource = StudentsList;
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

                    // Oid is GUID/uniqueidentifier, so use GetGuid and ToString
                    string query = "SELECT Oid, Name FROM Classe WHERE GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string oid = reader.GetGuid(0).ToString();
                            ClassesList.Add(new ClassItem
                            {
                                OID = oid,
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load classes: {ex.Message}", "OK");
            }
        }

        private async void OnClassSelected(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1) return;

            var selectedClass = ClassPicker.SelectedItem as ClassItem;
            await LoadStudents(selectedClass.OID);
        }

        private async System.Threading.Tasks.Task LoadStudents(string classOid)
        {
            StudentsList.Clear();

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT p.OID, p.FirstName, p.LastName 
                        FROM Profile p
                        INNER JOIN UserApp u ON u.Profile = p.OID
                        INNER JOIN AppRole r ON u.Role = r.OID
                        WHERE r.RoleName = 'Student' AND p.Classe = @ClassOID AND p.GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClassOID", new Guid(classOid)); // GUID param
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int oid = Convert.ToInt32(reader.GetValue(0));
                                StudentsList.Add(new Student2
                                {
                                    OID = oid,
                                    Name = $"{reader.GetString(1)} {reader.GetString(2)}"
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

        private async void OnAssignToDoClicked(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Validation Error", "Please select a class.", "OK");
                return;
            }
            if (StudentsList.Count == 0)
            {
                await DisplayAlert("Validation Error", "No students in the selected class.", "OK");
                return;
            }

            string workName = WorkNameEntry.Text;
            string workDescription = WorkDescriptionEditor.Text;
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date;

            if (string.IsNullOrWhiteSpace(workName))
            {
                await DisplayAlert("Validation Error", "Work name cannot be empty.", "OK");
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

                    foreach (var student in StudentsList)
                    {
                        string query = @"
                            INSERT INTO Todo (WorkName, WorkDescription, StartDate, EndDate, CreatedDate, CreatedBy, AssignedTo, IsCompleted, Status) 
                            VALUES (@workName, @workDescription, @startDate, @endDate, GETDATE(), @createdBy, @assignedTo, 0, 'Pending')";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@workName", workName);
                            cmd.Parameters.AddWithValue("@workDescription", string.IsNullOrWhiteSpace(workDescription) ? DBNull.Value : workDescription);
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);
                            cmd.Parameters.AddWithValue("@createdBy", TeacherOID); // Teacher's OID
                            cmd.Parameters.AddWithValue("@assignedTo", student.OID); // Student's OID

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await DisplayAlert("Success", "To-Do task assigned successfully to all students in the class!", "OK");

                // Clear inputs
                WorkNameEntry.Text = string.Empty;
                WorkDescriptionEditor.Text = string.Empty;
                StartDatePicker.Date = DateTime.Now;
                EndDatePicker.Date = DateTime.Now;
                ClassPicker.SelectedIndex = -1;
                StudentsList.Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to assign to-do task: {ex.Message}", "OK");
            }
        }
    }

    public class ClassItem
    {
        public string OID { get; set; }
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    public class Student2
    {
        public int OID { get; set; }
        public string Name { get; set; }
    }
}