
using System.Text.Json;

namespace SnapDoc;

internal class Functions
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<(double E, double N)> Wgs84ToLv95Async(double latitude, double longitude)
    {
        try
        {
            string url = $"https://geodesy.geo.admin.ch/reframe/wgs84tolv95?easting={longitude}&northing={latitude}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            double e, n;

            if (root.TryGetProperty("easting", out var eProp) && root.TryGetProperty("northing", out var nProp))
            {
                e = eProp.GetDouble();
                n = nProp.GetDouble();
            }
            else if (root.TryGetProperty("coordinates", out var coords) && coords.GetArrayLength() >= 2)
            {
                e = coords[0].GetDouble();
                n = coords[1].GetDouble();
            }
            else
            {
                throw new Exception("Unbekanntes REFRAME-Format");
            }

            return (e, n);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"REFRAME fehlgeschlagen ({ex.Message}), Fallback auf Näherungsformel...");
            return Wgs84ToLv95Approx(latitude, longitude);
        }
    }

    /// <summary>
    /// Näherungsformel von Dupraz/Marti (swisstopo 1999) – Genauigkeit ~1 m
    /// </summary>
    public static (double E, double N) Wgs84ToLv95Approx(double latitude, double longitude)
    {
        // Schritt 1: Umrechnung in Sexagesimalsekunden
        double latSec = latitude * 3600.0;
        double lonSec = longitude * 3600.0;

        // Schritt 2: Hilfsgrössen (Differenz zu Bern in 10000")
        double latAux = (latSec - 169028.66) / 10000.0;
        double lonAux = (lonSec - 26782.5) / 10000.0;

        // Schritt 3: LV95 berechnen (nach swisstopo-Dokument)
        double e = 2600072.37
                   + 211455.93 * lonAux
                   - 10938.51 * lonAux * latAux
                   - 0.36 * lonAux * latAux * latAux
                   - 44.54 * Math.Pow(lonAux, 3);

        double n = 1200147.07
                   + 308807.95 * latAux
                   + 3745.25 * lonAux * lonAux
                   + 76.63 * latAux * latAux
                   - 194.56 * lonAux * lonAux * latAux
                   + 119.79 * Math.Pow(latAux, 3);

        return (e, n);
    }
}
