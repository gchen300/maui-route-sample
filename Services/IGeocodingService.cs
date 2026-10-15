using MauiRouteSample.Models;
using System.Threading.Tasks;

namespace MauiRouteSample.Services;
public interface IGeocodingService
{
    // Returns the first matching location or null
    Task<LatLng?> GeocodeAsync(string address);
}
