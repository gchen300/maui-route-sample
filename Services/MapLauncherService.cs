using MauiRouteSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace MauiRouteSample.Services;
public class MapLauncherService : IMapLauncher
{
    public async Task LaunchNavigationAsync(LatLng origin, IReadOnlyList<LatLng> orderedWaypoints)
    {
        if (orderedWaypoints == null || orderedWaypoints.Count == 0)
            throw new ArgumentException("No waypoints to navigate to.", nameof(orderedWaypoints));

        var originParam = $"{origin.Latitude},{origin.Longitude}";
        var destination = orderedWaypoints.Last();
        var destinationParam = $"{destination.Latitude},{destination.Longitude}";

        // waypoints param: all ordered except the last (since destination is last)
        var waypointParts = orderedWaypoints.Take(orderedWaypoints.Count - 1)
                           .Select(p => $"{p.Latitude},{p.Longitude}");
        var waypointsParam = string.Join("|", waypointParts);

        var url = $"https://www.google.com/maps/dir/?api=1&origin={Uri.EscapeDataString(originParam)}&destination={Uri.EscapeDataString(destinationParam)}&travelmode=driving";
        if (!string.IsNullOrEmpty(waypointsParam))
            url += $"&waypoints={Uri.EscapeDataString(waypointsParam)}";

        await Launcher.OpenAsync(new Uri(url));
    }
}
