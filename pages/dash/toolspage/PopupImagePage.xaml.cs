namespace plz_fix.pages.dash.toolspage
{
    public partial class PopupImagePage : ContentPage
    {
        public PopupImagePage()
        {
            InitializeComponent();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Close the popup
        }
    }
}
