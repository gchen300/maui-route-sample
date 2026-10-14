namespace MauiRouteSample.Models;
public record LatLng(double Latitude, double Longitude)
{
    public override string ToString() => $"{Latitude},{Longitude}";
}
