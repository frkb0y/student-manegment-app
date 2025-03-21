using Newtonsoft.Json;
using ZXing;
using ZXing.Net.Maui.Controls;

namespace plz_fix.pages.login
{
    public partial class qrpage : ContentPage
    {
        private bool _isResultVisible = false;
        private bool _isScannerVisible = true;
        private string _school, _type, _section, _class;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public qrpage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // Properties to bind to the XAML
        public bool IsResultVisible
        {
            get => _isResultVisible;
            set
            {
                _isResultVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsScannerVisible
        {
            get => _isScannerVisible;
            set
            {
                _isScannerVisible = value;
                OnPropertyChanged();
            }
        }

        public string School
        {
            get => _school;
            set
            {
                _school = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string Section
        {
            get => _section;
            set
            {
                _section = value;
                OnPropertyChanged();
            }
        }

        public string Class
        {
            get => _class;
            set
            {
                _class = value;
                OnPropertyChanged();
            }
        }

        // Barcode detected
        private void barcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            var first = e.Results.FirstOrDefault();
            if (first != null)
            {
                ParseQRCode(first.Value);
                IsResultVisible = true;
                IsScannerVisible = false;
                barcodeReader.IsDetecting = false;
            }
        }

        private void ParseQRCode(string qrData)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<QRCodeData>(qrData);
                if (data != null)
                {
                    School = data.School;
                    Type = data.Type;
                    Section = data.Section;
                    Class = data.Class;
                }
                else
                {
                    School = "N/A";
                    Type = "N/A";
                    Section = "N/A";
                    Class = "N/A";
                }
            }
            catch
            {
                School = "Error";
                Type = "Error";
                Section = "Error";
                Class = "Error";
            }
        }

        // Retry Scan
        private void OnScanButtonClicked(object sender, EventArgs e)
        {
            IsResultVisible = false;
            IsScannerVisible = true;
            barcodeReader.IsDetecting = true;
        }

        // Join Button Clicked (Add navigation logic here)
        private async void OnJoinButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new loginpage());
        }
    }

    public class QRCodeData
    {
        public string School { get; set; }
        public string Type { get; set; }
        public string Section { get; set; }
        public string Class { get; set; }
    }
}
