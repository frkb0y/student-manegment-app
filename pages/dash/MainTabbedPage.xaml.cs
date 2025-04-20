namespace plz_fix.pages.dash
{
    public partial class MainTabbedPage : TabbedPage
    {
        private string _username;

        public MainTabbedPage(string username)
        {
            InitializeComponent();
            _username = username;

            // Pass username to each page that needs it
            Children.Clear();
            Children.Add(new index());
            Children.Add(new classes());
            Children.Add(new task());
            Children.Add(new chat());
            Children.Add(new profile(_username)); // <-- Pass username here
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
