#nullable disable

using PDFtoImage;
using SkiaSharp;
using System.Globalization;
using UraniumUI.Pages;


namespace bsm24.Views;
public partial class LoadPDFPages : UraniumContentPage
{
    public LoadPDFPages()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadPDFImages();
    }

    private async void LoadPDFImages()
    {
        List<FileItem> pdfImages = [];
        var result = await PickPdfFileAsync();

        busyOverlay.IsVisible = true;
        activityIndicator.IsRunning = true;
        busyText.Text = "PDF wird konvertiert...";

        await Task.Run(() =>
        {
            var root = GlobalJson.Data;
            byte[] bytearray = File.ReadAllBytes(result.FullPath);
            int pagecount = Conversion.GetPageCount(bytearray);

            var cacheDir = Path.Combine(FileSystem.AppDataDirectory, "cache");
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            for (int i = 0; i < pagecount; i++)
            {
                string imgPath = Path.Combine(FileSystem.AppDataDirectory, cacheDir, "plan_" + i + ".jpg");
                Conversion.SaveJpeg(imgPath, bytearray, i, options: new RenderOptions(Dpi: 300));

                // Bildgrösse auslesen
                var stream = File.OpenRead(imgPath);
                var skBitmap = SKBitmap.Decode(stream);
                Size _imgSize = new(skBitmap.Width, skBitmap.Height);

                pdfImages.Add(new FileItem
                {
                    ImagePath = imgPath,
                });
            }

            fileListView.ItemsSource = pdfImages;
            fileListView.Footer = pdfImages.Count + " Seite(n)";
        });

        activityIndicator.IsRunning = false;
        busyOverlay.IsVisible = false;
    }

    public static async Task<FileResult> PickPdfFileAsync()
    {
        try
        {
            // Öffne den FilePicker nur für PDF-Dateien
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Bitte wähle eine PDF-Datei aus",
                FileTypes = FilePickerFileType.Pdf // Nur PDF-Dateien anzeigen
            });

            if (fileResult != null)
                return fileResult;
        }
        catch (Exception ex)
        {
            // Fehlerbehandlung (z.B. wenn der Benutzer den Picker abbricht)
            Console.WriteLine($"Fehler beim Auswählen der Datei: {ex.Message}");
        }
        return null; // Kein PDF ausgewählt
    }

    private void AddPdfImages()
    {
        // Hauptverzeichnis, in dem die Suche beginnen soll (z.B. das App-Datenverzeichnis)
        string rootDirectory = FileSystem.AppDataDirectory;

        // Liste zum Speichern der gefundenen Dateien
        List<FileItem> foundFiles = [];

        string searchPattern = "*.json"; // Alle JSON-Dateien suchen

        // Alle Unterverzeichnisse und das Hauptverzeichnis durchsuchen
        try
        {
            // Rekursive Suche in allen Unterverzeichnissen
            string[] files = Directory.GetFiles(rootDirectory, searchPattern, SearchOption.AllDirectories);

            // Gefundene Dateien zur Liste hinzufügen
            foreach (var file in files)
            {
                string thumbImg = "banner_thumbnail.png";

                if (File.Exists(Path.Combine(Path.GetDirectoryName(file), "title_thumbnail.jpg")))
                    thumbImg = Path.Combine(Path.GetDirectoryName(file), "title_thumbnail.jpg");

                foundFiles.Add(new FileItem
                {
                    FileName = Path.GetFileNameWithoutExtension(file),
                    FilePath = file,
                    FileDate = "Geändert am:\n" + File.GetLastWriteTime(file).Date.ToString("d", new CultureInfo("de-DE")),
                    ImagePath = thumbImg
                });
            }
        }
        catch (Exception ex)
        {
            // Fehlerbehandlung, z.B. falls keine Zugriffsrechte auf bestimmte Verzeichnisse vorhanden sind
            Console.WriteLine("Fehler beim Durchsuchen der Verzeichnisse: " + ex.Message);
        }

        // Liste der JSON-Dateien dem ListView zuweisen
        fileListView.ItemsSource = foundFiles;
        fileListView.Footer = foundFiles.Count + " Projekte";
    }

}
