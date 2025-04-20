using LocalizationResourceManager.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif

namespace plz_fix.pages.login
{
    public partial class start : ContentPage
    {
        private readonly ILocalizationResourceManager _localizationResourceManager;

        public start(ILocalizationResourceManager localizationResourceManager)
        {
            InitializeComponent();
            _localizationResourceManager = localizationResourceManager;
            BindingContext = this;
        }

        private async void OnGetStartedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new qrpage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Shell.SetNavBarIsVisible(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            // ? Ask for notification permission if needed (Android 13+)
            await RequestNotificationPermissionAsync();
        }

        private async Task RequestNotificationPermissionAsync()
        {
#if ANDROID
            try
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
                {
                    var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

                    if (AndroidX.Core.Content.ContextCompat.CheckSelfPermission(activity, Android.Manifest.Permission.PostNotifications)
                        != Android.Content.PM.Permission.Granted)
                    {
                        AndroidX.Core.App.ActivityCompat.RequestPermissions(
                            activity,
                            new string[] { Android.Manifest.Permission.PostNotifications },
                            1001
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Permission Error", "Could not request notification permission: " + ex.Message, "OK");
            }
#endif
            await Task.CompletedTask;
        }

    }
}
