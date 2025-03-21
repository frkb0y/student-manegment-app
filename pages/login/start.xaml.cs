using LocalizationResourceManager.Maui;
using Microsoft.Maui.Controls;

namespace plz_fix.pages.login
{
    public partial class start : ContentPage
    {
        private readonly ILocalizationResourceManager _localizationResourceManager;

        public start(ILocalizationResourceManager localizationResourceManager)
        {
            InitializeComponent();
            _localizationResourceManager = localizationResourceManager;
            BindingContext = this;
        }

        private async void OnGetStartedClicked(object sender, EventArgs e)
        {
            // Navigate to the login page or another page
            await Navigation.PushAsync(new qrpage());
        }

    

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}