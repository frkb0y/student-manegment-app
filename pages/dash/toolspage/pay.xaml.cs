using Microsoft.Maui.Controls;
using plz_fix.pages.dash.toolspage;
using System.Collections.ObjectModel;

namespace plz_fix.pages.dash
{
    public partial class pay : ContentPage
    {
        public ObservableCollection<PaymentRecord> PaymentRecords { get; set; }

        public pay()
        {
            InitializeComponent();
            var data = PaymentData.GetPayments();
            PaymentRecords = new ObservableCollection<PaymentRecord>(data);
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
