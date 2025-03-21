namespace plz_fix.pages.dash;

public partial class MainTabbedPage : TabbedPage
{
    public MainTabbedPage()
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