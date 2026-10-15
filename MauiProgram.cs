using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MauiRouteSample.Services;
using System.Net.Http;
using CommunityToolkit.Maui;

namespace MauiRouteSample;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .ConfigureFonts(fonts => { /* add fonts if desired */ });

        // Register pages & services
        builder.Services.AddSingleton<MainPage>();

        // Replace with your Google Directions & Geocoding API key (must have Directions & Geocoding APIs enabled)
        const string googleApiKey = "YOUR_GOOGLE_API_KEY";

        builder.Services.AddSingleton<IRouteService>(sp =>
            new GoogleDirectionsService(new HttpClient(), googleApiKey));
        builder.Services.AddSingleton<IGeocodingService>(sp =>
            new GoogleGeocodingService(new HttpClient(), googleApiKey));
        builder.Services.AddSingleton<IMapLauncher, MapLauncherService>();

        return builder.Build();
    }
}
