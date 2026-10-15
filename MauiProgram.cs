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

        builder.Services.AddSingleton<MainPage>();

        const string googleDirectionsApiKey = "YOUR_GOOGLE_DIRECTIONS_API_KEY";

        builder.Services.AddSingleton<IRouteService>(sp =>
            new GoogleDirectionsService(new HttpClient(), googleDirectionsApiKey));
        builder.Services.AddSingleton<IMapLauncher, MapLauncherService>();

        return builder.Build();
    }
}
