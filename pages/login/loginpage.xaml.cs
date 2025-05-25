using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;

namespace plz_fix.pages.login
{
    public partial class loginpage : ContentPage
    {
        public loginpage()
        {
            InitializeComponent();
        }

        private void OnEyeIconClicked(object sender, EventArgs e)
        {
            // Toggle the IsPassword property of the Entry
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

            // Optionally, change the icon to reflect the state
            var button = (Button)sender;

            // Check the theme and change the icon accordingly
            if (PasswordEntry.IsPassword)
            {
                button.ImageSource = App.Current.RequestedTheme == AppTheme.Dark
                    ? "icons/eyes/eye_light.png"  // For light theme, closed eye icon
                    : "icons/eyes/eye_dark.png";   // For dark theme, closed eye icon
            }
            else
            {
                button.ImageSource = App.Current.RequestedTheme == AppTheme.Dark
                    ? "icons/eyes/eye_light_closed.png"  // For light theme, open eye icon
                    : "icons/eyes/eye_dark_closed.png";   // For dark theme, open eye icon
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            // Read user input
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Username and password cannot be empty", "OK");
                return;
            }

            // SQL Server connection details
            string srvrname = "DESKTOP-V0T4PRO\\USER19";  // Your server name
            string srvrdbname = "XAF";  // Your database name
            string srvruser = "sa";  // Your SQL username
            string srvrpass = "123456789";  // Your SQL password

            string sqlconn = $"Data Source={srvrname};Initial Catalog={srvrdbname};User Id={srvruser};Password={srvrpass};TrustServerCertificate=True;Encrypt=True;";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(sqlconn))
                {
                    await sqlConnection.OpenAsync();

                    // Query to fetch the user's role
                    string query = "SELECT Role FROM XAF.dbo.UserApp WHERE Username=@Username AND Password=@Password";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        // Add the parameters
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);  // Adjust this depending on how your passwords are stored

                        var role = await cmd.ExecuteScalarAsync();

                        if (role != null)
                        {
                            int userRole = Convert.ToInt32(role);

                            // Navigate based on the user's role
                            if (userRole == 3) // Admin
                            {
                                await Navigation.PushAsync(new plz_fix.pages.controllerp.adminp());
                            }
                            else if (userRole == 1) // Student
                            {
                                await Navigation.PushAsync(new plz_fix.pages.dash.MainTabbedPage(username));
                            }
                            else if (userRole == 2) // Teacher
                            {
                                await Navigation.PushAsync(new plz_fix.pages.controllerp.teatcherp(username));
                            }
                            else
                            {
                                await DisplayAlert("Error", "Invalid Role", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid Credentials", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Database Error", ex.Message, "OK");
            }
        }
    }
}