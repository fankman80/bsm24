#nullable disable

#if ANDROID
using Android.Webkit;
#endif

using Microsoft.Maui.Platform;
using System.Text;

namespace bsm24.Views;

public partial class MapView
{
    public MapView()
    {
        InitializeComponent();
        //ShowCurrentLocationOnMap();
        LoadMap(7.429563, 46.946480, "https://map.geo.admin.ch/api/icons/sets/default/icons/001-marker@1x-255,0,0.png");

        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Settings.SetGeolocationEnabled(true);
            handler.PlatformView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            handler.PlatformView.Settings.JavaScriptEnabled = true;
            handler.PlatformView.SetWebChromeClient(new MyWebChromeClient());
#endif
        });
    }

    public void LoadMap(double longitude, double latitude, string iconUrl)
    {
        string kml = GenerateKml(longitude, latitude, iconUrl);
        string base64Kml = ConvertKmlToBase64(kml);

        string htmlContent = LoadHtmlWithKml(base64Kml);
        var htmlSource = new HtmlWebViewSource { Html = htmlContent };
        GeoAdminWebView.Source = htmlSource;
    }

    public string ConvertKmlToBase64(string kml)
    {
        var bytes = Encoding.UTF8.GetBytes(kml);
        return Convert.ToBase64String(bytes);
    }

    public string LoadHtmlWithKml(string base64Kml)
    {
        string htmlTemplate = File.ReadAllText("map_template.html");
        return htmlTemplate.Replace("{{KML_DATA}}", base64Kml);
    }

    public string GenerateKml(double longitude, double latitude, string iconUrl)
    {
        return null;
        //return $"<kml xmlns="http://www.opengis.net/kml/2.2" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.opengis.net/kml/2.2 https://developers.google.com/kml/schema/kml22gx.xsd\"><Document><name>Zeichnung</name><Placemark id=\"drawing_feature_1737384612504\"><name></name><Style><IconStyle><scale>1.125</scale><Icon><href>https://map.geo.admin.ch/api/icons/sets/default/icons/001-marker@1x-255,0,0.png</href><gx:w>48</gx:w><gx:h>48</gx:h></Icon><hotSpot x=\"24\" y=\"6\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle><LabelStyle><color>ff0000ff</color><scale>1.5</scale></LabelStyle></Style><ExtendedData><Data name=\"textOffset\"><value>0,-44.75</value></Data><Data name=\"type\"><value>marker</value></Data></ExtendedData><Point><coordinates>{longitude},{latitude}</coordinates></Point></Placemark></Document></kml>";
    }


    private async void ShowCurrentLocationOnMap()
    {
        var location = await Helper.GetCurrentLocationAsync();

        if (location != null)
        {
            Functions.LLtoSwissGrid(location.Latitude, location.Longitude, out double swissEasting, out double swissNorthing);
            string formattedEasting = swissEasting.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string formattedNorthing = swissNorthing.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string geoAdminUrl = $"https://map.geo.admin.ch/#/map?lang=de&center={formattedEasting},{formattedNorthing}&z=12";
            GeoAdminWebView.Source = geoAdminUrl;
        }
        else
            GeoAdminWebView.Source = "https://map.geo.admin.ch";
    }
}

#if ANDROID
internal class MyWebChromeClient : WebChromeClient
{
    public static async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status == PermissionStatus.Granted)
            return status;
        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            return status;
        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            // Prompt the user with additional information as to why the permission is needed
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        return status;
    }


    public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
    {
        base.OnGeolocationPermissionsShowPrompt(origin, callback);
        callback.Invoke(origin, true, false);
    }
}
#endif