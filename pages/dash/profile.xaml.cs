using LocalizationResourceManager.Maui;

namespace plz_fix.pages.dash;

public partial class profile : ContentPage
{
    private ILocalizationResourceManager _localizationResourceManager;

    public profile()
	{
		InitializeComponent();
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
}