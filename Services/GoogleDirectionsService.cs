using MauiRouteSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiRouteSample.Services;
public class GoogleDirectionsService : IRouteService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    public GoogleDirectionsService(HttpClient http, string apiKey)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<IReadOnlyList<LatLng>> GetOptimizedWaypointsAsync(LatLng origin, IReadOnlyList<LatLng> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0) return Array.Empty<LatLng>();

        // Build waypoints parameter with optimize:true
        string waypointsParam = "optimize:true|" + string.Join("|", waypoints.Select(w => $"{w.Latitude},{w.Longitude}"));

        // Use origin -> destination same as origin (we only need waypoint_order)
        string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin.Latitude},{origin.Longitude}" +
                     $"&destination={origin.Latitude},{origin.Longitude}" +
                     $"&waypoints={Uri.EscapeDataString(waypointsParam)}" +
                     $"&departure_time=now&traffic_model=best_guess&key={_apiKey}";

        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;

        if (root.TryGetProperty("status", out var status) && status.GetString() != "OK")
        {
            var msg = root.TryGetProperty("error_message", out var err) ? err.GetString() : status.GetString();
            throw new Exception($"Directions API error: {msg}");
        }

        var routes = root.GetProperty("routes");
        if (routes.GetArrayLength() == 0) return waypoints;

        var route0 = routes[0];
        var waypointOrder = new List<int>();
        if (route0.TryGetProperty("waypoint_order", out var orderJson))
        {
            foreach (var idx in orderJson.EnumerateArray())
                waypointOrder.Add(idx.GetInt32());
        }
        else
        {
            // If API didn't return a waypoint_order, just return original order
            return waypoints;
        }

        var ordered = waypointOrder.Select(i => waypoints[i]).ToList();
        return ordered;
    }
}
