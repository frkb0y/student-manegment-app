using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageAccountsPage : ContentPage
    {
        public ObservableCollection<AccountModel> AccountsList { get; set; } = new ObservableCollection<AccountModel>();
        public ObservableCollection<RoleModel> RolesList { get; set; } = new ObservableCollection<RoleModel>();
        public ObservableCollection<ProfileModel2> ProfilesList { get; set; } = new ObservableCollection<ProfileModel2>();
        private int? editingOid = null;

        public ManageAccountsPage()
        {
            InitializeComponent();
            AccountsCollectionView.ItemsSource = AccountsList;
            RolePicker.ItemsSource = RolesList;
            RolePicker.ItemDisplayBinding = new Binding("RoleName");
            ProfilePicker.ItemsSource = ProfilesList;
            ProfilePicker.ItemDisplayBinding = new Binding("ProfileName");
            LoadRoles();
            LoadProfiles();
            LoadAccounts();
        }

        private async void LoadRoles()
        {
            try
            {
                RolesList.Clear();
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT OID, RoleName FROM AppRole WHERE GCRecord IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RolesList.Add(new RoleModel
                            {
                                OID = reader.GetInt32(0),
                                RoleName = reader.GetString(1)
                            });
                        }
                    }
                }
                RolePicker.ItemsSource = RolesList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load roles: {ex.Message}", "OK");
            }
        }

        private async void LoadProfiles()
        {
            try
            {
                ProfilesList.Clear();
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT OID, FirstName, LastName FROM Profile WHERE GCRecord IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProfilesList.Add(new ProfileModel2
                            {
                                OID = reader.GetInt32(0),
                                ProfileName = $"{reader.GetString(1)} {reader.GetString(2)}"
                            });
                        }
                    }
                }
                ProfilePicker.ItemsSource = ProfilesList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profiles: {ex.Message}", "OK");
            }
        }

        private async void LoadAccounts()
        {
            try
            {
                AccountsList.Clear();
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = @"SELECT u.OID, u.Username, u.Password, u.Role, u.Profile, r.RoleName, p.FirstName, p.LastName
                                     FROM UserApp u
                                     LEFT JOIN AppRole r ON u.Role = r.OID
                                     LEFT JOIN Profile p ON u.Profile = p.OID
                                     WHERE u.GCRecord IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AccountsList.Add(new AccountModel
                            {
                                OID = reader.GetInt32(0),
                                Username = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Password = reader.IsDBNull(2) ? null : reader.GetString(2),
                                RoleOID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                ProfileOID = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                RoleName = reader.IsDBNull(5) ? null : reader.GetString(5),
                                ProfileName = (reader.IsDBNull(6) ? "" : reader.GetString(6)) +
                                              (reader.IsDBNull(7) ? "" : " " + reader.GetString(7))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load accounts: {ex.Message}", "OK");
            }
        }

        private async void OnSaveAccountClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();
            var selectedRole = RolePicker.SelectedItem as RoleModel;
            var selectedProfile = ProfilePicker.SelectedItem as ProfileModel2;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || selectedRole == null || selectedProfile == null)
            {
                await DisplayAlert("Validation Error", "All fields are required.", "OK");
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    if (editingOid == null)
                    {
                        string query = @"INSERT INTO UserApp (Username, Password, Role, Profile, OptimisticLockField) 
                                         VALUES (@username, @password, @role, @profile, 1)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@role", selectedRole.OID);
                            cmd.Parameters.AddWithValue("@profile", selectedProfile.OID);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        string query = @"UPDATE UserApp SET Username=@username, Password=@password, Role=@role, Profile=@profile 
                                         WHERE OID=@oid";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@role", selectedRole.OID);
                            cmd.Parameters.AddWithValue("@profile", selectedProfile.OID);
                            cmd.Parameters.AddWithValue("@oid", editingOid.Value);
                            await cmd.ExecuteNonQueryAsync();
                        }
                        editingOid = null;
                    }
                }
                ClearFields();
                LoadAccounts();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save account: {ex.Message}", "OK");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            ClearFields();
            editingOid = null;
        }

        private void ClearFields()
        {
            UsernameEntry.Text = "";
            PasswordEntry.Text = "";
            RolePicker.SelectedIndex = -1;
            ProfilePicker.SelectedIndex = -1;
        }

        private void OnEditAccountClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is AccountModel account)
            {
                editingOid = account.OID;
                UsernameEntry.Text = account.Username;
                PasswordEntry.Text = account.Password;
                RolePicker.SelectedItem = RolesList.FirstOrDefault(r => r.OID == account.RoleOID);
                ProfilePicker.SelectedItem = ProfilesList.FirstOrDefault(p => p.OID == account.ProfileOID);
            }
        }

        private async void OnDeleteAccountClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is AccountModel account)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete account '{account.Username}'?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        string query = "UPDATE UserApp SET GCRecord = 1 WHERE OID=@oid";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@oid", account.OID);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    LoadAccounts();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete account: {ex.Message}", "OK");
                }
            }
        }
    }

    public class AccountModel
    {
        public int OID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? RoleOID { get; set; }
        public int? ProfileOID { get; set; }
        public string RoleName { get; set; }
        public string ProfileName { get; set; }
    }

    public class RoleModel
    {
        public int OID { get; set; }
        public string RoleName { get; set; }
    }

    public class ProfileModel2
    {
        public int OID { get; set; }
        public string ProfileName { get; set; }
    }
}