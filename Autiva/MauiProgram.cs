using Autiva.Helpers;
using Autiva.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;

namespace Autiva;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        SyncfusionLicenseProvider.RegisterLicense("DEIN_SYNCFUSION_KEY_HIER_EINFÜGEN");

        builder.Services.AddSingleton<AutivaDb>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        ServiceLocator.Services = app.Services;
        return app;
    }
}
