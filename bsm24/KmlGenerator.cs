#nullable disable
using SharpKml.Base;
using SharpKml.Dom;
using Placemark = SharpKml.Dom.Placemark;
using Point = SharpKml.Dom.Point;

namespace bsm24;

public class KmlGenerator
{
    public static void GenerateKml(string filePath, List<(double Latitude, double Longitude, string Name, DateTime time)> coordinates)
    {
        // Erstellen des KML-Dokuments
        var document = new Document
        {
            Name = "Koordinaten"
        };

        // Hinzufügen von Placemarks für jede Koordinate
        foreach (var (Latitude, Longitude, Name, Time) in coordinates)
        {
            var point = new Point
            {
                Coordinate = new Vector(Latitude, Longitude)
            };

            var timeStamp = new SharpKml.Dom.Timestamp
            {
                When = Time
            };

            var placemark = new Placemark
            {
                Name = Name,
                Geometry = point,
                Time = timeStamp,
            };

            document.AddFeature(placemark);
        }

        // Das KML-Dokument als root-Element setzen
        var kml = new Kml { Feature = document };

        // Serialisieren und in die Datei schreiben
        var serializer = new Serializer();
        serializer.Serialize(kml);

        // Die KML-Daten in die angegebene Datei schreiben
        File.WriteAllText(filePath, serializer.Xml);
    }
}

