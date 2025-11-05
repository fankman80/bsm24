#nullable disable
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using SnapDoc.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnapDoc.ViewModels;

public partial class GPSViewModel : INotifyPropertyChanged
{
    public static GPSViewModel Instance { get; } = new GPSViewModel();
    public event PropertyChangedEventHandler PropertyChanged;

    private string _gpsData;
    private FontImageSource _gpsButtonIcon;
    private double _lon;
    private double _lat;
    private double _acc;

    public string GPSData
    {
        get => _gpsData;
        set { _gpsData = value; OnPropertyChanged(); }
    }

    public double Lon
    {
        get => _lon;
        set { _lon = value; OnPropertyChanged(); }
    }

    public double Lat
    {
        get => _lat;
        set { _lat = value; OnPropertyChanged(); }
    }

    public double Acc
    {
        get => _acc;
        set { _acc = value; OnPropertyChanged(); }
    }

    public bool IsRunning { get; set; }

    public FontImageSource GPSButtonIcon
    {
        get => _gpsButtonIcon;
        set { _gpsButtonIcon = value; OnPropertyChanged(); }
    }

    public Command ToggleGPSCommand { get; }

    private CancellationTokenSource _gpsToken;

    private GPSViewModel()
    {
        ToggleGPSCommand = new Command(OnToggleGPS);
        GPSButtonIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Location_off,
            Color = Application.Current.RequestedTheme == AppTheme.Dark
                    ? (Color)Application.Current.Resources["PrimaryDark"]
                    : (Color)Application.Current.Resources["Primary"],
            Size = 24
        };
    }

    private async void OnToggleGPS(object obj)
    {
        await Toggle(!IsRunning);

        GPSButtonIcon = new FontImageSource
        {
            FontFamily = "MaterialOutlined",
            Glyph = IsRunning
                    ? UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Where_to_vote
                    : UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Location_off,
            Color = Application.Current.RequestedTheme == AppTheme.Dark
                    ? (Color)Application.Current.Resources["PrimaryDark"]
                    : (Color)Application.Current.Resources["Primary"],
            Size = 24
        };
    }

    public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Aktiviert/deaktiviert die Standortabfrage.
    /// </summary>
    public async Task<bool> Toggle(bool isOn)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            GPSData = "Standortberechtigung verweigert.";
            return false;
        }

        if (!isOn)
        {
            _gpsToken?.Cancel();
            _gpsToken = null;
            GPSData = string.Empty;
            IsRunning = false;
        }
        else
        {
            _gpsToken = new CancellationTokenSource();
            IsRunning = true;
            _ = Task.Run(() => StartListeningAsync(_gpsToken.Token));
        }

        return true;
    }

    /// <summary>
    /// Wiederholte Standortabfrage (simuliert Listener).
    /// </summary>
    private async Task StartListeningAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var request = new GeolocationRequest(
                    GeolocationAccuracy.High,
                    TimeSpan.FromSeconds(SettingsService.Instance.GpsMinTimeUpdate));

                var location = await Geolocation.Default.GetLocationAsync(request, token);

                if (location != null)
                {
                    Lat = location.Latitude;
                    Lon = location.Longitude;
                    Acc = location.Accuracy ?? 0;

                    GPSData = string.Format(
                        "Zeit: {0}\nLat: {1:F6}\nLon: {2:F6}\nGenauigkeit: {3:F1} m\nQuelle: {4}",
                        location.Timestamp.LocalDateTime,
                        Lat,
                        Lon,
                        Acc,
                        location.Source);
                }
            }
            catch (Exception ex)
            {
                GPSData = $"Fehler: {ex.Message}";
            }

            // Wartezeit zwischen Updates
            await Task.Delay(TimeSpan.FromSeconds(SettingsService.Instance.GpsMinTimeUpdate), token);
        }
    }

    /// <summary>
    /// Liefert den letzten bekannten Standort (ohne neue Abfrage).
    /// </summary>
    public async Task<Location> GetLastKnownLocation()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                return null;

            var location = await Geolocation.Default.GetLastKnownLocationAsync();
            if (location != null)
            {
                Lat = location.Latitude;
                Lon = location.Longitude;
                Acc = location.Accuracy ?? 0;
                GPSData = $"Letzter Standort: {Lat:F6}, {Lon:F6} (±{Acc:F1}m)";
            }

            return location;
        }
        catch (Exception ex)
        {
            GPSData = $"Fehler: {ex.Message}";
            return null;
        }
    }
}
