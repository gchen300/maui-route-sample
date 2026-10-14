# MauiRouteSample

A .NET MAUI sample that:
- Lets the user tap up to 4 pickup locations on a map.
- Calls Google Directions API with `optimize:true` and `departure_time=now` to compute an optimized visiting order considering real-time traffic.
- Opens the native map/navigation app (Google Maps via URL) with the ordered waypoints for turn-by-turn navigation.

Prerequisites
- .NET MAUI workload installed (Visual Studio 2022/2023 or later with MAUI support).
- Google Directions API key (enable Directions API in Google Cloud).
- Google Maps API key for Android map tiles if you want map rendering on Android (add to AndroidManifest meta-data).
- On iOS you may optionally use Apple Maps; this sample opens a Google Maps URL which works on iOS if Google Maps is installed or via web fallback.

Setup
1. Replace placeholders:
   - In `MauiProgram.cs` set `YOUR_GOOGLE_DIRECTIONS_API_KEY`.
   - In `Platforms/Android/AndroidManifest.xml` set `YOUR_GOOGLE_MAPS_API_KEY` (if you want Android map tiles).
2. Restore packages and build.
3. Deploy to a device (recommended) or emulator (ensure Google Play/Maps available for Android emulator).
4. Give the app location permission when prompted. Tap map to add pickups (max 4). Tap "Calculate & Navigate" to compute optimized sequence and open navigation.

Notes
- Directions API requests may incur charges. Use API key restrictions.
- If you prefer another provider (Mapbox, TomTom), replace GoogleDirectionsService with that provider's route/optimization API.
- For production, store API keys securely (not in source).
