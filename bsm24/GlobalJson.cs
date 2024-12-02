#nullable disable

using bsm24.Models;
using System.Text.Json;

namespace bsm24;

public static class GlobalJson
{
    private static JsonDataModel _userData = new();
    private static string _filePath;

    public static JsonDataModel Data
    {
        get => _userData;
        set => _userData = value;
    }

    public static JsonSerializerOptions GetOptions()
    {
        return new() { WriteIndented = true };
    }

    public static string ToJson(JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(_userData, options);
    }

    public static void FromJson(string json)
    {
        _userData = JsonSerializer.Deserialize<JsonDataModel>(json);
    }

    public static void SaveToFile()
    {
        if (string.IsNullOrEmpty(_filePath))
        {
            Console.WriteLine("Kein Dateipfad festgelegt. Laden Sie zuerst eine Datei.");
            return;
        }

        try
        {
            string json = ToJson(GetOptions());
            json = json.Replace("\r\n", "\n").Replace("\r", "\n"); // Zeilenumbrüche für Android anpassen

            File.WriteAllText(_filePath, json); // Überschreibt die Datei mit neuen Daten
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Speichern der Datei: {ex.Message}");
        }
    }

    public static void LoadFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                _filePath = filePath; // Speichere den Dateipfad
                string json = File.ReadAllText(filePath);
                FromJson(json);
            }
            else
            {
                Console.WriteLine($"Datei existiert nicht: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Laden der Datei: {ex.Message}");
        }
    }

    public static void CreateNewFile(string filePath)
    {
        try
        {
            filePath = GetUniqueFilePath(filePath);

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Directory.CreateDirectory(Path.Combine(directoryPath, "images"));
                Directory.CreateDirectory(Path.Combine(directoryPath, "images", "originals"));
                Directory.CreateDirectory(Path.Combine(directoryPath, "plans"));
                Directory.CreateDirectory(Path.Combine(directoryPath, "thumbnails"));
            }

            File.WriteAllText(filePath, ""); // Erstellt eine neue Datei;
            _filePath = filePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Erstellen der Datei: {ex.Message}");
        }
    }

    public static String GetFilePath()
    {
        return _filePath;
    }

    private static string GetUniqueFilePath(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;
        string newFilePath = filePath;

        // Prüfe, ob die Datei existiert und hänge fortlaufend eine Nummer an
        while (File.Exists(newFilePath))
        {
            newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension} ({counter}){extension}");
            counter++;
        }

        return newFilePath;
    }
}