using Microsoft.Maui.Controls;

namespace plz_fix.pages.dash
{
    public partial class pay : ContentPage
    {
        public pay()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void OnSubscribeClicked(object sender, System.EventArgs e)
        {
            // Handle subscription logic here
            DisplayAlert("Subscription", "Thank you for subscribing!", "OK");
        }
    }
}