using Microsoft.Maui.Controls;
using System;

namespace plz_fix.pages.controllerp
{
    public partial class adminp : ContentPage
    {
        public adminp()
        {
            InitializeComponent();
        }

        private async void OnManageAccountsClicked(object sender, EventArgs e)
        {
            // Navigate to the page for managing accounts
            await Navigation.PushAsync(new function.ManageAccountsPage());
        }

        private async void OnManageProfilesClicked(object sender, EventArgs e)
        {
            // Navigate to the page for managing profiles
            await Navigation.PushAsync(new function.ManageProfilesPage());
        }

        private async void OnManageRolesClicked(object sender, EventArgs e)
        {
            // Navigate to the page for managing roles
            await Navigation.PushAsync(new function.ManageRolesPage());
        }
    }
}