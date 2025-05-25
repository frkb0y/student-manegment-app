using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace plz_fix.pages.controllerp
{
    public partial class teatcherp : ContentPage
    {
        private string _username;
        public teatcherp(string username)
        {
            InitializeComponent();
            _username = username;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTeacherInfo();
        }

        private async Task LoadTeacherInfo()
        {
            string sqlconn = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User Id=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconn))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT TOP 1 
                            P.FirstName, 
                            P.LastName, 
                            P.BirthDate, 
                            P.Address, 
                            P.ContactInfo, 
                            S.Name AS SubjectName,
                            C.Name AS ClassName
                        FROM [UserApp] U
                        INNER JOIN [Profile] P ON U.Profile = P.OID
                        LEFT JOIN [Subject] S ON P.Subject = S.OID
                        LEFT JOIN [Classe] C ON P.Classe = C.OID
                        WHERE U.Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                                string className = reader["ClassName"] != DBNull.Value ? reader["ClassName"].ToString() : "N/A";
                                string subjectName = reader["SubjectName"] != DBNull.Value ? reader["SubjectName"].ToString() : "N/A";

                                TeacherNameLabel.Text = fullName;
                                TeacherSubjectLabel.Text = $"Subject: {subjectName}";
                                TeacherContactLabel.Text = $"Class: {className}";
                                TeacherBirthDateLabel.Text = reader["BirthDate"] != DBNull.Value ?
                                    $"Birth Date: {Convert.ToDateTime(reader["BirthDate"]).ToShortDateString()}" : "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not load teacher info: " + ex.Message, "OK");
            }
        }

        private async void OnManageNotesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new function.ManageNotesPage());
        }

        private async void OnManageSubjectsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new function.ManageSubjectsPage());
        }

        private async void OnManageToDoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new function.ManageToDoPage());
        }

        private async void OnManageEventsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new function.ManageEventsPage());
        }

        private async void OnManageAbsencesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new function.ManageAbsencesPage());
        }
    }
}