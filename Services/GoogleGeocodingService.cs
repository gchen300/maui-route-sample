using MauiRouteSample.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiRouteSample.Services;
public class GoogleGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GoogleGeocodingService(HttpClient http, string apiKey)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<LatLng?> GeocodeAsync(string address)
    {
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        if (root.TryGetProperty("status", out var status) && status.GetString() != "OK")
            return null;

        var results = root.GetProperty("results");
        if (results.GetArrayLength() == 0) return null;
        var loc = results[0].GetProperty("geometry").GetProperty("location");
        var lat = loc.GetProperty("lat").GetDouble();
        var lng = loc.GetProperty("lng").GetDouble();
        return new LatLng(lat, lng);
    }
}
