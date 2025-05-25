using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Data.SqlClient;
using System;
using System.Windows.Input;

namespace plz_fix.pages.dash
{
    public partial class classes : ContentPage
    {
        private string _username;

        public classes(string username)
        {
            InitializeComponent();
            _username = username;
            BindingContext = new SubjectViewModel(_username, Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }

    public class SubjectViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Subject> Subjects { get; set; }
        public ICommand SubjectTappedCommand { get; set; }

        private readonly string _username;
        private readonly INavigation _navigation;

        public SubjectViewModel(string username, INavigation navigation)
        {
            _username = username;
            _navigation = navigation;
            Subjects = new ObservableCollection<Subject>();
            SubjectTappedCommand = new Command<Subject>(OnSubjectTapped);
            LoadSubjects();
        }

        private async void OnSubjectTapped(Subject subject)
        {
            if (subject != null)
            {
                // Pass OID and Name to the AttendancePage
                await _navigation.PushAsync(new AttendancePage(subject.OID, subject.Name, _username));
            }
        }

        private async void LoadSubjects()
        {
            string connectionString = "Server=DESKTOP-V0T4PRO\\USER19;Database=XAF;User Id=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT OID, Name FROM Subject";  // Also fetching OID now
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Subjects.Add(new Subject
                            {
                                OID = Convert.ToInt32(reader["OID"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Subject
    {
        public int OID { get; set; }  // OID is added for identification
        public string Name { get; set; }
    }

    public class RandomColorConverter : IValueConverter
    {
        private Random _random = new Random();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Returns a random color
            return Color.FromRgb(_random.Next(256), _random.Next(256), _random.Next(256));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not used for this case
            throw new NotImplementedException();
        }
    }
}
