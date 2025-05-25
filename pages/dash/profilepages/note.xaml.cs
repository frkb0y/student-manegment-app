using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace plz_fix.pages.dash.profilepages
{
    public partial class note : ContentPage
    {
        private string _username;
        private ObservableCollection<Grouping<string, NoteModel>> noteGroups = new ObservableCollection<Grouping<string, NoteModel>>();
        private int _previousNoteCount = 0;
        private Timer _refreshTimer;

        public note(string username)
        {
            InitializeComponent();
            _username = username;
            NotesCollectionView.ItemsSource = noteGroups;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            StartAutoRefresh();
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
                        SELECT N.Score, S.Name AS SubjectName, NS.Name AS NatureSeanceName
                        FROM Note N
                        INNER JOIN Subject S ON S.OID = N.Subject
                        LEFT JOIN NatureSeance NS ON NS.OID = N.NatureSeance
                        INNER JOIN Profile P ON N.Student = P.OID
                        INNER JOIN UserApp U ON P.OID = U.Profile
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
                                    Score = Convert.ToDecimal(reader["Score"]),
                                    NatureSeanceName = reader["NatureSeanceName"] == DBNull.Value ? "Cour" : reader["NatureSeanceName"].ToString()
                                });
                            }

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

                            // Group notes by NatureSeance
                            var grouped = newNotes
                                .GroupBy(n => n.NatureSeanceName)
                                .Select(g => new Grouping<string, NoteModel>(g.Key, g))
                                .ToList();

                            // Update UI
                            noteGroups.Clear();
                            foreach (var group in grouped)
                                noteGroups.Add(group);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load notes: " + ex.Message, "OK");
            }
        }

        public static class NoteData
        {
            public static int GetNoteCount(string username)
            {
                int count = 0;
                string sqlconn = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User Id=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
                try
                {
                    using (SqlConnection conn = new SqlConnection(sqlconn))
                    {
                        conn.Open();
                        string query = @"
                            SELECT COUNT(*) 
                            FROM Note N
                            INNER JOIN Profile P ON N.Student = P.OID
                            INNER JOIN UserApp U ON P.OID = U.Profile
                            WHERE U.Username = @Username";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            count = (int)cmd.ExecuteScalar();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get note count: " + ex.Message);
                }
                return count;
            }
        }

        public class NoteModel
        {
            public string SubjectName { get; set; }
            public decimal Score { get; set; }
            public string NatureSeanceName { get; set; }
        }

        // Helper class for grouping
        public class Grouping<K, T> : ObservableCollection<T>
        {
            public K Key { get; }
            public Grouping(K key, IEnumerable<T> items) : base(items)
            {
                Key = key;
            }
        }
    }
}