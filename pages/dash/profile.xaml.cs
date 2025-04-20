using LocalizationResourceManager.Maui;

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
    }
}
