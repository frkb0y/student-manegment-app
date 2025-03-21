using LocalizationResourceManager.Maui;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using plz_fix;
using plz_fix.Resources;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseLocalizationResourceManager(settings =>
            {
                settings.RestoreLatestCulture(true);
                settings.AddResource(plz_fix.Resources.Lang.Resource.ResourceManager);
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<plz_fix.pages.setting>();
        builder.Services.AddTransient<plz_fix.pages.login.start>();
        builder.Services.AddTransient<plz_fix.pages.login.qrpage>();
        return builder.Build();
    }
}
