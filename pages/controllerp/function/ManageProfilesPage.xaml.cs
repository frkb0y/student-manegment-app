using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System;

namespace plz_fix.pages.controllerp.function
{
    public partial class ManageProfilesPage : ContentPage
    {
        public ObservableCollection<ProfileModel> ProfilesList { get; set; } = new ObservableCollection<ProfileModel>();
        private int? editingOid = null;

        public ManageProfilesPage()
        {
            InitializeComponent();
            ProfilesCollectionView.ItemsSource = ProfilesList;
            LoadProfiles();
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
                    string query = "SELECT OID, FirstName, LastName, BirthDate, Address, ContactInfo, SubjectExpertise FROM Profile WHERE GCRecord IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ProfilesList.Add(new ProfileModel
                            {
                                OID = reader.GetInt32(0),
                                FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                                LastName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                BirthDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ContactInfo = reader.IsDBNull(5) ? null : reader.GetString(5),
                                SubjectExpertise = reader.IsDBNull(6) ? null : reader.GetString(6),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profiles: {ex.Message}", "OK");
            }
        }

        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            string firstName = FirstNameEntry.Text?.Trim();
            string lastName = LastNameEntry.Text?.Trim();
            DateTime? birthDate = BirthDatePicker.IsVisible ? (DateTime?)BirthDatePicker.Date : null;
            string address = AddressEntry.Text?.Trim();
            string contactInfo = ContactInfoEntry.Text?.Trim();
            string subjectExpertise = SubjectExpertiseEntry.Text?.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                await DisplayAlert("Validation Error", "First name and last name are required.", "OK");
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
                        string query = @"INSERT INTO Profile (FirstName, LastName, BirthDate, Address, ContactInfo, SubjectExpertise, OptimisticLockField) 
                                         VALUES (@firstName, @lastName, @birthDate, @address, @contactInfo, @subjectExpertise, 1)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@firstName", firstName);
                            cmd.Parameters.AddWithValue("@lastName", lastName);
                            if (birthDate.HasValue)
                                cmd.Parameters.AddWithValue("@birthDate", birthDate.Value);
                            else
                                cmd.Parameters.AddWithValue("@birthDate", DBNull.Value);
                            cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(address) ? DBNull.Value : address);
                            cmd.Parameters.AddWithValue("@contactInfo", string.IsNullOrWhiteSpace(contactInfo) ? DBNull.Value : contactInfo);
                            cmd.Parameters.AddWithValue("@subjectExpertise", string.IsNullOrWhiteSpace(subjectExpertise) ? DBNull.Value : subjectExpertise);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        string query = @"UPDATE Profile SET FirstName=@firstName, LastName=@lastName, BirthDate=@birthDate, Address=@address, ContactInfo=@contactInfo, SubjectExpertise=@subjectExpertise 
                                         WHERE OID=@oid";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@firstName", firstName);
                            cmd.Parameters.AddWithValue("@lastName", lastName);
                            if (birthDate.HasValue)
                                cmd.Parameters.AddWithValue("@birthDate", birthDate.Value);
                            else
                                cmd.Parameters.AddWithValue("@birthDate", DBNull.Value);
                            cmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(address) ? DBNull.Value : address);
                            cmd.Parameters.AddWithValue("@contactInfo", string.IsNullOrWhiteSpace(contactInfo) ? DBNull.Value : contactInfo);
                            cmd.Parameters.AddWithValue("@subjectExpertise", string.IsNullOrWhiteSpace(subjectExpertise) ? DBNull.Value : subjectExpertise);
                            cmd.Parameters.AddWithValue("@oid", editingOid.Value);
                            await cmd.ExecuteNonQueryAsync();
                        }
                        editingOid = null;
                    }
                }
                ClearFields();
                LoadProfiles();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save profile: {ex.Message}", "OK");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            ClearFields();
            editingOid = null;
        }

        private void ClearFields()
        {
            FirstNameEntry.Text = "";
            LastNameEntry.Text = "";
            BirthDatePicker.Date = DateTime.Today;
            AddressEntry.Text = "";
            ContactInfoEntry.Text = "";
            SubjectExpertiseEntry.Text = "";
        }

        private void OnEditProfileClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ProfileModel profile)
            {
                editingOid = profile.OID;
                FirstNameEntry.Text = profile.FirstName;
                LastNameEntry.Text = profile.LastName;
                if (profile.BirthDate.HasValue)
                {
                    BirthDatePicker.IsVisible = true;
                    BirthDatePicker.Date = profile.BirthDate.Value;
                }
                else
                {
                    BirthDatePicker.IsVisible = false;
                }
                AddressEntry.Text = profile.Address;
                ContactInfoEntry.Text = profile.ContactInfo;
                SubjectExpertiseEntry.Text = profile.SubjectExpertise;
            }
        }

        private async void OnDeleteProfileClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ProfileModel profile)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete profile '{profile.FirstLastName}'?", "Yes", "No");
                if (!confirm) return;

                try
                {
                    string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        await conn.OpenAsync();
                        string query = "UPDATE Profile SET GCRecord = 1 WHERE OID=@oid";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@oid", profile.OID);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    LoadProfiles();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to delete profile: {ex.Message}", "OK");
                }
            }
        }
    }

    public class ProfileModel
    {
        public int OID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
        public string SubjectExpertise { get; set; }
        public string FirstLastName => $"{FirstName} {LastName}";
        public string BirthDateString => BirthDate.HasValue ? BirthDate.Value.ToString("yyyy-MM-dd") : "";
    }
}