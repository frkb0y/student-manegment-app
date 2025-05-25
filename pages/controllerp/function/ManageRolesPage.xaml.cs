using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageRolesPage : ContentPage
    {
        public ObservableCollection<AppRole> RolesList { get; set; } = new ObservableCollection<AppRole>();

        public ManageRolesPage()
        {
            InitializeComponent();
            RolesCollectionView.ItemsSource = RolesList;
            LoadRoles();
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
                            RolesList.Add(new AppRole
                            {
                                OID = reader.GetInt32(0),
                                RoleName = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load roles: {ex.Message}", "OK");
            }
        }

        private async void OnAddRoleClicked(object sender, EventArgs e)
        {
            string newRoleName = NewRoleEntry.Text?.Trim();
            if (string.IsNullOrEmpty(newRoleName))
            {
                await DisplayAlert("Validation Error", "Role name cannot be empty.", "OK");
                return;
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO AppRole (RoleName, OptimisticLockField) VALUES (@roleName, 1)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@roleName", newRoleName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                NewRoleEntry.Text = string.Empty;
                LoadRoles();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add role: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteRoleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is AppRole role)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete role '{role.RoleName}'?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        string query = "UPDATE AppRole SET GCRecord = 1 WHERE OID = @roleId";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@roleId", role.OID);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    LoadRoles();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete role: {ex.Message}", "OK");
                }
            }
        }

        private async void OnEditRoleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is AppRole role)
            {
                string result = await DisplayPromptAsync("Edit Role", $"Edit role name for '{role.RoleName}':", initialValue: role.RoleName);
                if (string.IsNullOrWhiteSpace(result) || result == role.RoleName) return;

                try
                {
                    string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        string query = "UPDATE AppRole SET RoleName = @roleName WHERE OID = @roleId";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@roleName", result.Trim());
                            cmd.Parameters.AddWithValue("@roleId", role.OID);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    LoadRoles();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to edit role: {ex.Message}", "OK");
                }
            }
        }
    }

    public class AppRole
    {
        public int OID { get; set; }
        public string RoleName { get; set; }
    }
}