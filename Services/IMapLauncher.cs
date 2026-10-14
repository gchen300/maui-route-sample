using MauiRouteSample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiRouteSample.Services;
public interface IMapLauncher
{
    // origin: current location; orderedWaypoints: the optimized sequence to visit
    Task LaunchNavigationAsync(LatLng origin, IReadOnlyList<LatLng> orderedWaypoints);
}
