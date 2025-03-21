namespace plz_fix.pages.dash;

public partial class classes : ContentPage
{
	public classes()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }
}