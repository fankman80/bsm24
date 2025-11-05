#nullable disable
using Microsoft.Maui.ApplicationModel;
using Shiny.Locations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnapDoc.ViewModels;

public partial class GPSViewModel : INotifyPropertyChanged
{
    public static GPSViewModel Instance { get; } = new GPSViewModel();
    public event PropertyChangedEventHandler PropertyChanged;

    public event Action<GpsReading> LocationUpdated;

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

    private string _gpsButtonText = "AUS";
    public string GPSButtonText
    {
        get => _gpsButtonText;
        set { _gpsButtonText = value; OnPropertyChanged(); }
    }

    public Command ToggleGPSCommand { get; }

    private IGpsManager _gpsManager;

    private GPSViewModel()
    {
        ToggleGPSCommand = new Command(OnToggleGPS);
        UpdateGPSButtonIcon();
        _gpsManager = Shiny.ShinyHost.Resolve<IGpsManager>();
    }

    private async void OnToggleGPS(object obj)
    {
        GPSButtonText = IsRunning ? "AUS" : "initialisiere...";
        UpdateGPSButtonIcon();

        await Toggle(!IsRunning);
    }

    private void UpdateGPSButtonIcon()
    {
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
    /// Startet oder stoppt den Shiny-GPS-Service.
    /// </summary>
    public async Task Toggle(bool isOn)
    {
        if (!await _gpsManager.RequestAccess().ConfigureAwait(false))
        {
            GPSData = "Standortberechtigung verweigert.";
            GPSButtonText = "AUS";
            return;
        }

        if (!isOn)
        {
            await _gpsManager.StopListener();
            IsRunning = false;
            GPSButtonText = "AUS";
        }
        else
        {
            var request = new GpsRequest
            {
                Accuracy = GpsAccuracy.High,
                Interval = TimeSpan.FromSeconds(SettingsService.Instance.GpsMinTimeUpdate),
                ThrottledInterval = TimeSpan.FromSeconds(SettingsService.Instance.GpsMinTimeUpdate),
                UseBackground = false
            };

            await _gpsManager.StartListener(request);
            IsRunning = true;
            GPSButtonText = "AN";
        }

        UpdateGPSButtonIcon();
    }

    /// <summary>
    /// Wird von LocationDelegate bei jedem neuen GPS-Update aufgerufen.
    /// </summary>
    public void OnGpsReading(GpsReading reading)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Lat = reading.Position.Latitude;
            Lon = reading.Position.Longitude;
            Acc = reading.Position.Accuracy ?? 0;

            GPSData = string.Format(
                "Zeit: {0}\nLat: {1:F6}\nLon: {2:F6}\nGenauigkeit: {3:F1} m",
                reading.Timestamp.LocalDateTime,
                Lat,
                Lon,
                Acc);

            GPSButtonText = "AN";
            LocationUpdated?.Invoke(reading);
        });
    }
}
