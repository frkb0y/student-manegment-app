namespace plz_fix.pages.dash;

public partial class chat : ContentPage
{
	public chat()
	{
		InitializeComponent();
        BindingContext = new ChatViewModel();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }
}