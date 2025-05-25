using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace plz_fix.pages.dash
{
    public partial class AttendancePage : ContentPage
    {
        public ObservableCollection<AppointmentAttendance> Attendances { get; set; }
        private string _username;
        private int _subjectOid;

        public AttendancePage(int subjectOid, string subjectName, string username)
        {
            InitializeComponent();
            _subjectOid = subjectOid;
            _username = username;
            Attendances = new ObservableCollection<AppointmentAttendance>();
            BindingContext = this;
            Title = subjectName; // Set the title to subject name
            LoadAttendanceData(); // Load data when the page is initialized
        }

        private async void LoadAttendanceData()
        {
            string connectionString = "Server=DESKTOP-V0T4PRO\\USER19;Database=XAF;User Id=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    // JOIN with Classe, Salle, NatureSeance to get names
                    string query = @"
                        SELECT 
                            aa.Oid, 
                            aa.Content, 
                            c.Name AS ClassRoomName,
                            ns.Name AS NatureSeanceName, 
                            s.Name AS SalleName, 
                            aa.StartDate, 
                            aa.EndDate, 
                            aa.Student, 
                            aa.AttendanceStatus, 
                            aa.OptimisticLockField, 
                            aa.GCRecord
                        FROM AppointmentAttendance aa
                        LEFT JOIN Classe c ON c.Oid = aa.ClassRoom
                        LEFT JOIN NatureSeance ns ON ns.Oid = aa.NatureSeance
                        LEFT JOIN Salle s ON s.Oid = aa.Salle
                        WHERE aa.Content = @SubjectOid";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubjectOid", _subjectOid); // Use parameterized query to avoid SQL injection

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Attendances.Add(new AppointmentAttendance
                                {
                                    Oid = reader["Oid"]?.ToString(),
                                    Content = reader["Content"]?.ToString(),
                                    ClassRoom = reader["ClassRoomName"]?.ToString(), // Show name
                                    NatureSeance = reader["NatureSeanceName"]?.ToString(), // Show name
                                    Salle = reader["SalleName"]?.ToString(), // Show name
                                    StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd HH:mm") : string.Empty,
                                    EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd HH:mm") : string.Empty,
                                    Student = reader["Student"] != DBNull.Value ? Convert.ToInt32(reader["Student"]) : 0,
                                    AttendanceStatus = reader["AttendanceStatus"]?.ToString(),
                                    OptimisticLockField = reader["OptimisticLockField"]?.ToString(),
                                    GCRecord = reader["GCRecord"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class AppointmentAttendance
    {
        public string Oid { get; set; } // GUID
        public string Content { get; set; } // GUID
        public string ClassRoom { get; set; } // Name instead of ID
        public string NatureSeance { get; set; } // Name instead of ID
        public string Salle { get; set; } // Name instead of ID
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Student { get; set; }
        public string AttendanceStatus { get; set; }
        public string OptimisticLockField { get; set; }
        public string GCRecord { get; set; }
    }
}