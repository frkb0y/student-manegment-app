namespace plz_fix.pages.dash
{
    public partial class MainTabbedPage : TabbedPage
    {
        private string _username;

        public MainTabbedPage(string username)
        {
            InitializeComponent();
            _username = username;

            // Add all tabs
            Children.Clear();
            Children.Add(new index(_username) { IconImageSource = "icons/dashicon/dashbord.png", Title = "" });
            Children.Add(new classes(_username) { IconImageSource = "icons/dashicon/classes.png", Title = "" });
            Children.Add(new task(_username) { IconImageSource = "icons/dashicon/task.png", Title = "" });
            Children.Add(new chat() { IconImageSource = "icons/dashicon/chat.png", Title = "" });
            Children.Add(new profile(_username) { IconImageSource = "icons/dashicon/profile.png", Title = "" });



            // Listen for tab switch messages
            MessagingCenter.Subscribe<index>(this, "GoToClassesTab", (sender) =>
            {
                this.CurrentPage = this.Children[1]; // 1 = classes
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
