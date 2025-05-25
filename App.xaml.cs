using LocalizationResourceManager.Maui;
using Microsoft.Maui.Controls;
using Plugin.Firebase.CloudMessaging;

namespace plz_fix
{
    public partial class App : Application
    {
        private readonly ILocalizationResourceManager _localizationResourceManager;

        public App(ILocalizationResourceManager localizationResourceManager)
        {
            _localizationResourceManager = localizationResourceManager;
            InitializeComponent();

            // Set the main page of the app with a NavigationPage wrapper
            var navigationPage = new NavigationPage(new pages.login.loginpage());
            
            // Hide the navigation bar globally for all pages
            navigationPage.Pushed += (sender, e) =>
            {
                if (e.Page is Page page)
                {
                    Shell.SetNavBarIsVisible(page, false);
                    NavigationPage.SetHasNavigationBar(page, false);
                }
            };

            MainPage = navigationPage;
        }
    }
}