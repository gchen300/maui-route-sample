using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MauiRouteSample.Services;
using System.Net.Http;

namespace MauiRouteSample;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps() // enable maps
            .ConfigureFonts(fonts => { /* add fonts if desired */ });

        // Register pages & services
        builder.Services.AddSingleton<MainPage>();

        // Replace with your Google Directions API key (must have Directions API enabled)
        const string googleDirectionsApiKey = "YOUR_GOOGLE_DIRECTIONS_API_KEY";

        builder.Services.AddSingleton<IRouteService>(sp =>
            new GoogleDirectionsService(new HttpClient(), googleDirectionsApiKey));
        builder.Services.AddSingleton<IMapLauncher, MapLauncherService>();

        return builder.Build();
    }
}
