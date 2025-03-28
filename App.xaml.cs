using LocalizationResourceManager.Maui;
using Microsoft.Maui.Controls;

namespace plz_fix
{
    public partial class App : Application
    {
        private readonly ILocalizationResourceManager _localizationResourceManager;

        public App(ILocalizationResourceManager localizationResourceManager)
        {
            _localizationResourceManager = localizationResourceManager;
            InitializeComponent();

            MainPage = new NavigationPage(new pages.dash.MainTabbedPage());

        }

      
    }
}
