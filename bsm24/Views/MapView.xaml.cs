#nullable disable

#if ANDROID
using Android.Webkit;
#endif

using System.Globalization;
using bsm24.Services;
using bsm24.Models;
using Mopups.Services;

namespace bsm24.Views;

public partial class MapView : IQueryAttributable
{
    public string PlanId;
    public string PinId;
    public MapView()
    {
#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
            handler.PlatformView.Settings.JavaScriptEnabled = true;
            handler.PlatformView.Settings.DomStorageEnabled = true;
            handler.PlatformView.Settings.SetGeolocationEnabled(true);
            handler.PlatformView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            handler.PlatformView.Settings.AllowContentAccess = true;
            handler.PlatformView.Settings.AllowFileAccess = true;
            handler.PlatformView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
            handler.PlatformView.SetWebViewClient(new CustomWebViewClient());
        });
#endif
        InitializeComponent();
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("planId", out object value1))
            PlanId = value1 as string;
        if (query.TryGetValue("pinId", out object value2))
            PinId = value2 as string;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        double lon, lat, zoom;

        if (PinId != null && GlobalJson.Data.Plans[PlanId].Pins[PinId].GeoLocation != null)
        {
            lon = GlobalJson.Data.Plans[PlanId].Pins[PinId].GeoLocation.WGS84.Longitude;
            lat = GlobalJson.Data.Plans[PlanId].Pins[PinId].GeoLocation.WGS84.Latitude;
            zoom = 18;
        }
        else
        {             
            var location = await Helper.GetCurrentLocationAsync();
            if (location != null)
            {
                lon = location.Longitude;
                lat = location.Latitude;
                zoom = 18;
            }
            else
            {
                lon = 8.226692;
                lat = 46.80121;
                zoom = 8;
            }
        }

        var htmlSource = new HtmlWebViewSource
        {
            Html = LoadHtmlFromFile(lon, lat, zoom),
            BaseUrl = "https://"
        };

        GeoAdminWebView.Source = htmlSource;

#if WINDOWS
        GeoAdminWebView.Navigated += (s, e) =>
        {
            GeoAdminWebView.EvaluateJavaScriptAsync(Generatescript());
        };
#endif
    }


#if ANDROID
    public class CustomWebViewClient : WebViewClient
    {
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(Generatescript(), null);
        }
    }
#endif

    private static string LoadHtmlFromFile(double lon, double lat, double zoom)
    {
        // Lade das HTML-Template
        var assembly = typeof(MapView).Assembly;
        using var stream = assembly.GetManifestResourceStream("bsm24.Resources.Raw.index.html");
        using var reader = new StreamReader(stream);
        string htmlContent = reader.ReadToEnd();

        // Ersetze die Platzhalter für die Koordinaten im HTML
        string _center_koord = lon.ToString(CultureInfo.InvariantCulture) + ", " + lat.ToString(CultureInfo.InvariantCulture);
        string _zoom = zoom.ToString();
        
        htmlContent = htmlContent.Replace("{center_koord}", _center_koord);
        htmlContent = htmlContent.Replace("{mapzoom}", _zoom);
        htmlContent = htmlContent.Replace("{icon}", SettingsService.Instance.IconUrls[SettingsService.Instance.MapIcon]);
        htmlContent = htmlContent.Replace("{iconzoom}", ((double)SettingsService.Instance.MapIconSize / 100).ToString(CultureInfo.InvariantCulture));

        return htmlContent;
    }

    private static string Generatescript()
    {
        string positionsJson = "[";
        foreach (var plan in GlobalJson.Data.Plans)
        {
            if (GlobalJson.Data.Plans[plan.Key].Pins != null)
            {
                foreach (var pin in GlobalJson.Data.Plans[plan.Key].Pins)
                {
                    if (GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].GeoLocation != null)
                    {
                        var lon = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].GeoLocation.WGS84.Longitude;
                        var lat = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].GeoLocation.WGS84.Latitude;
                        var pindesc = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinDesc;
                        var pinlocation = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinLocation;
                        var pinname = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinName;
                        positionsJson += $"{{ lon: {lon.ToString(CultureInfo.InvariantCulture)}, lat: {lat.ToString(CultureInfo.InvariantCulture)}, pinname: '{pinname}', pinlocation: '{pinlocation}', pindesc: '{pindesc}'}},";
                    }
                }
            }
        }
        positionsJson = positionsJson.TrimEnd(',') + "]";
        return $"setMultipleMarkers({positionsJson});";
    }
    private async void SetPosClicked(object sender, EventArgs e)
    {
        var popup = new PopupDualResponse("Sind Sie sicher dass Sie die Positionsdaten überschreiben wollen?");
        await MopupService.Instance.PushAsync(popup);
        var result = await popup.PopupDismissedTask;
        if (result != null)
        {
            var location = await Helper.GetCurrentLocationAsync();
            GlobalJson.Data.Plans[PlanId].Pins[PinId].GeoLocation = location != null ? new GeoLocData(location) : null;
            GeoAdminWebView.Reload();
        }
    }
}