using LocalizationResourceManager.Maui;
using Microsoft.Maui.Controls;

using System.Globalization;
using System.Resources;

namespace plz_fix.pages
{
    public partial class setting : ContentPage
    {
        ILocalizationResourceManager _localizationResourceManager;

        public setting(ILocalizationResourceManager localizationResourceManager)
        {
            InitializeComponent();
            _localizationResourceManager = localizationResourceManager;
            BindingContext = this;

            // Initialize the switch based on the current theme
            ThemeSwitch.IsToggled = Application.Current.RequestedTheme == AppTheme.Dark;

            // Set the initial selected index for the language picker
            switch (_localizationResourceManager.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    LanguagePicker.SelectedIndex = 0;
                    break;
                case "es":
                    LanguagePicker.SelectedIndex = 1;
                    break;
                case "fr":
                    LanguagePicker.SelectedIndex = 2;
                    break;
                default:
                    LanguagePicker.SelectedIndex = 0;
                    break;
            }
        }

        private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }

          protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private void OnLanguagePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLanguage = LanguagePicker.SelectedItem.ToString();
            if (selectedLanguage == "English")
            {
                _localizationResourceManager.CurrentCulture = new CultureInfo("en");
            }
            else if (selectedLanguage == "Español")
            {
                _localizationResourceManager.CurrentCulture = new CultureInfo("es");
            }
            else if (selectedLanguage == "Français")
            {
                _localizationResourceManager.CurrentCulture = new CultureInfo("fr");
            }
        }
    }
}
