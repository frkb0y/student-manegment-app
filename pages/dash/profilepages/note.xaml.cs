using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace plz_fix.pages.dash.profilepages
{
    public partial class note : ContentPage
    {
        private string _username;
        private ObservableCollection<NoteModel> notes = new ObservableCollection<NoteModel>();
        private int _previousNoteCount = 0;
        private Timer _refreshTimer;

        public note(string username)
        {
            InitializeComponent();
            _username = username;
            NotesCollectionView.ItemsSource = notes;
            StartAutoRefresh();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            _ = LoadNotes();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _refreshTimer?.Dispose();
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new Timer(async _ =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await LoadNotes();
                });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        private async Task LoadNotes()
        {
            string sqlconn = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User Id=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconn))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT N.Score, S.Name AS SubjectName
                        FROM Note N
                        INNER JOIN [UserApp] U ON N.Student = U.OID
                        INNER JOIN [Subject] S ON S.OID = N.Subject
                        WHERE U.Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", _username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var newNotes = new List<NoteModel>();

                            while (await reader.ReadAsync())
                            {
                                newNotes.Add(new NoteModel
                                {
                                    SubjectName = reader["SubjectName"].ToString(),
                                    Score = Convert.ToDecimal(reader["Score"])
                                });
                            }

                            // If note count has increased, notify
                            if (newNotes.Count > _previousNoteCount)
                            {
                                await LocalNotificationCenter.Current.Show(new NotificationRequest
                                {
                                    NotificationId = 100,
                                    Title = "New Note",
                                    Description = "A new note has been added.",
                                    ReturningData = "Dummy",
                                    Schedule = new NotificationRequestSchedule
                                    {
                                        NotifyTime = DateTime.Now
                                    }
                                });

                            }

                            _previousNoteCount = newNotes.Count;

                            // Update observable collection
                            notes.Clear();
                            foreach (var note in newNotes)
                                notes.Add(note);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load notes: " + ex.Message, "OK");
            }
        }
    }

    public class NoteModel
    {
        public string SubjectName { get; set; }
        public decimal Score { get; set; }
    }
}
