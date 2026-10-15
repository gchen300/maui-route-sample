using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using MauiRouteSample.Models;
using MauiRouteSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MauiRouteSample;
public partial class MainPage : ContentPage
{
    readonly List<string> _addresses = new();
    readonly List<LatLng> _points = new();
    readonly IRouteService _routeService;
    readonly IMapLauncher _mapLauncher;
    readonly IGeocodingService _geocoder;

    public MainPage(IRouteService routeService, IMapLauncher mapLauncher, IGeocodingService geocoder)
    {
        InitializeComponent();

        _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
        _mapLauncher = mapLauncher ?? throw new ArgumentNullException(nameof(mapLauncher));
        _geocoder = geocoder ?? throw new ArgumentNullException(nameof(geocoder));

        RefreshPickupList();
    }

    private async void AddAddress_Clicked(object sender, EventArgs e)
    {
        var address = AddressEntry?.Text?.Trim();
        if (string.IsNullOrEmpty(address))
        {
            await DisplayAlert("Empty", "Please enter an address or place name.", "OK");
            return;
        }

        if (_points.Count >= 4)
        {
            await DisplayAlert("Limit", "You can add up to 4 pickup locations only.", "OK");
            return;
        }

        try
        {
            var loc = await _geocoder.GeocodeAsync(address);
            if (loc == null)
            {
                await DisplayAlert("Not found", "Could not geocode the address.", "OK");
                return;
            }

            _addresses.Add(address);
            _points.Add(loc);
            RefreshPickupList();
            AddressEntry.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void RefreshPickupList()
    {
        PickupContainer.Children.Clear();

        if (_addresses.Count == 0)
        {
            PickupContainer.Children.Add(new Label
            {
                Text = "No pickups added yet.",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            });
            return;
        }

        for (int i = 0; i < _addresses.Count; i++)
        {
            PickupContainer.Children.Add(new Label
            {
                Text = $"{i + 1}. {_addresses[i]}",
                FontAttributes = FontAttributes.Bold
            });
        }
    }

    private void Clear_Clicked(object sender, EventArgs e)
    {
        _addresses.Clear();
        _points.Clear();
        RefreshPickupList();
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        if (_points.Count == 0)
        {
            await DisplayAlert("No pickups", "Add one or more pickup locations first.", "OK");
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

            await _mapLauncher.LaunchNavigationAsync(origin, ordered);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
