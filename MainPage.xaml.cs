using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using MauiRouteSample.Models;
using MauiRouteSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiRouteSample;
public partial class MainPage : ContentPage
{
    readonly List<Pin> _pins = new();
    readonly List<LatLng> _points = new();
    readonly IRouteService _routeService;
    readonly IMapLauncher _mapLauncher;

    public MainPage(IRouteService routeService, IMapLauncher mapLauncher)
    {
        InitializeComponent();

        _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
        _mapLauncher = mapLauncher ?? throw new ArgumentNullException(nameof(mapLauncher));

        Map.MapClicked += Map_MapClicked;

        // center to a sensible default (Seattle) until we obtain location
        Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Microsoft.Maui.Controls.Maps.Location(47.6062, -122.3321), Distance.FromKilometers(5)));
    }

    private void Map_MapClicked(object sender, MapClickedEventArgs e)
    {
        if (_points.Count >= 4)
        {
            DisplayAlert("Limit", "You can add up to 4 pickup locations only.", "OK");
            return;
        }

        var latlng = new LatLng(e.Location.Latitude, e.Location.Longitude);
        _points.Add(latlng);

        var pin = new Pin
        {
            Label = $"Pickup {_points.Count}",
            Location = new Microsoft.Maui.Controls.Maps.Location(latlng.Latitude, latlng.Longitude),
            Type = PinType.Place
        };
        _pins.Add(pin);
        Map.Pins.Add(pin);
    }

    private void Clear_Clicked(object sender, EventArgs e)
    {
        foreach (var p in _pins) Map.Pins.Remove(p);
        _pins.Clear();
        _points.Clear();
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        if (_points.Count == 0)
        {
            await DisplayAlert("No pickups", "Add one or more pickup locations first by tapping the map.", "OK");
            return;
        }

        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var geo = await Geolocation.Default.GetLocationAsync(request);

            if (geo == null)
            {
                await DisplayAlert("Location unavailable", "Could not get current location.", "OK");
                return;
            }

            var origin = new LatLng(geo.Latitude, geo.Longitude);

            var ordered = await _routeService.GetOptimizedWaypointsAsync(origin, _points);

            // update labels on pins to show sequence
            for (int i = 0; i < ordered.Count; i++)
            {
                var o = ordered[i];
                var match = _pins.FirstOrDefault(p => Math.Abs(p.Location.Latitude - o.Latitude) < 1e-6 && Math.Abs(p.Location.Longitude - o.Longitude) < 1e-6);
                if (match != null) match.Label = $"Stop {i + 1}";
            }

            await _mapLauncher.LaunchNavigationAsync(origin, ordered);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
