using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace plz_fix.pages.dash
{
    public partial class pay : ContentPage
    {
        public ObservableCollection<PaymentRecord> PaymentRecords { get; set; }

        public pay()
        {
            InitializeComponent();
            PaymentRecords = new ObservableCollection<PaymentRecord>
            {
                new PaymentRecord { Description = "School Fees - Term 1", Amount = "200 TND", Date = "Jan 15, 2024", Status = "Paid", StatusColor = "Green" },
                new PaymentRecord { Description = "School Fees - Term 2", Amount = "200 TND", Date = "Apr 15, 2024", Status = "Pending", StatusColor = "Red" }
            };
            BindingContext = this; // Bind directly to this class
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }

    public class PaymentRecord
    {
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }
}
