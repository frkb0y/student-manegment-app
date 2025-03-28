namespace plz_fix.pages.dash;

public partial class index : ContentPage
{
    public index()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnPayButtonClicked(object sender, EventArgs e)
    {
       
        await Navigation.PushAsync(new pay());
    }
}