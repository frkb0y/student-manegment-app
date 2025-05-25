using LocalizationResourceManager.Maui;
using Microsoft.Data.SqlClient;
namespace plz_fix.pages.dash
{
    public partial class profile : ContentPage
    {
        private ILocalizationResourceManager _localizationResourceManager;
        private string _username;

        public profile(string username)
        {
            InitializeComponent();
            _username = username;
            Usernamen.Text = $"{_username}";
            LoadStudentInfo();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new plz_fix.pages.setting(_localizationResourceManager));
        }

        private async void noteClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new profilepages.note(_username)); // <-- Pass username here
        }



        private async void LoadStudentInfo()
        {
            string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                SELECT U.Username, S.Name
                FROM UserApp U
                INNER JOIN Student S ON U.Student = S.OID
                WHERE U.Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string fullName = reader["Name"].ToString();
                                string username = reader["Username"].ToString();

                                Usernamen.Text = fullName;          // Top label (big font)
                                UsernameTag.Text = "@" + username;  // Smaller label below
                            }
                            else
                            {
                                await DisplayAlert("Not Found", "User profile not found.", "OK");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
        }




    }
}
