using MauiRouteSample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiRouteSample.Services;
public interface IRouteService
{
    // origin = current location, waypoints = selected pickups
    // returns waypoints in optimized visiting order (list of LatLng)
    Task<IReadOnlyList<LatLng>> GetOptimizedWaypointsAsync(LatLng origin, IReadOnlyList<LatLng> waypoints);
}
