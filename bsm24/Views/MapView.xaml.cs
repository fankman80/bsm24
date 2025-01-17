#nullable disable

using System.ComponentModel;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace bsm24.Views;

public partial class MapView
{
    public MapView()
    {
        InitializeComponent();
        ShowCurrentLocationOnMap();
    }

    private async void ShowCurrentLocationOnMap()
    {
        var location = await GetCurrentLocationAsync();

        if (location != null)
        {
            Functions.LLtoSwissGrid(location.Latitude, location.Longitude, out double swissEasting, out double swissNorthing);
            string formattedEasting = swissEasting.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string formattedNorthing = swissNorthing.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string geoAdminUrl = $"https://map.geo.admin.ch/#/map?lang=de&center={formattedEasting},{formattedNorthing}&z=12";

            GeoAdminWebView.Source = geoAdminUrl;
        }
        else
        {
            GeoAdminWebView.Source = "https://map.geo.admin.ch";
        }
    }

    public async Task<Location> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }

            return location;
        }
        catch (Exception ex)
        {
            // Fehlerbehandlung
            Console.WriteLine($"Unable to get location: {ex.Message}");
            return null;
        }
    }
}