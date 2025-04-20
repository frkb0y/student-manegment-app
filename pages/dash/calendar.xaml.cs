using Syncfusion.Maui.Scheduler;
using Microsoft.Maui.Controls;
using System;

namespace plz_fix.pages.dash
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();
            // You can customize the calendar here if needed.
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
