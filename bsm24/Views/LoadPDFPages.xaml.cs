#nullable disable

using bsm24.Models;
using bsm24.Services;
using PDFtoImage;
using SkiaSharp;

namespace bsm24.Views;
public partial class LoadPDFPages : ContentPage
{
    IEnumerable<FileResult> resultList;
    public int DynamicSpan { get; set; } = 0; // Standardwert
    private int targetDpi = SettingsService.Instance.PdfQuality;

    public LoadPDFPages()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadPreviewPDFImages();
    }
    protected override bool OnBackButtonPressed()
    {
        // Zurück-Taste ignorieren
        return true;
    }

    private async void LoadPreviewPDFImages()
    {
        resultList = await PickPdfFileAsync(); // neue Methode gibt IEnumerable<FileResult> zurück
        if (resultList != null && resultList.Any())
        {
            List<ImageItem> pdfImages = [];
            busyOverlay.IsOverlayVisible = true;
            busyOverlay.IsActivityRunning = true;
            busyOverlay.BusyMessage = "Lade PDF Seiten...";

            await Task.Run(() =>
            {
                if (!Directory.Exists(Settings.CacheDirectory))
                    Directory.CreateDirectory(Settings.CacheDirectory);

                int pdfIndex = 0;

                foreach (var file in resultList)
                {
                    byte[] bytearray = File.ReadAllBytes(file.FullPath);
                    int pagecount = Conversion.GetPageCount(bytearray);

                    for (int i = 0; i < pagecount; i++)
                    {
                        string imgBaseName = $"pdf_{pdfIndex}_page_{i}";
                        string imgPath = Path.Combine(Settings.DataDirectory, Settings.CacheDirectory, imgBaseName + ".jpg");
                        string previewPath = Path.Combine(Settings.DataDirectory, Settings.CacheDirectory, "preview_" + imgBaseName + ".jpg");

                        var renderOptions = new RenderOptions
                        {
                            AntiAliasing = PdfAntiAliasing.None,
                            Dpi = 72,
                            WithAnnotations = false,
                            WithFormFill = false,
                        };

                        Conversion.SaveJpeg(previewPath, bytearray, i, options: renderOptions);

                        using var stream = File.OpenRead(previewPath);
                        using var skBitmap = SKBitmap.Decode(stream);

                        int widthHighDpi = skBitmap.Width * targetDpi / 72;
                        int heightHighDpi = skBitmap.Height * targetDpi / 72;

                        if (widthHighDpi > Settings.MaxPdfImageSizeW || heightHighDpi > Settings.MaxPdfImageSizeH)
                        {
                            widthHighDpi = targetDpi * Settings.MaxPdfImageSizeW / widthHighDpi;
                            heightHighDpi = targetDpi * Settings.MaxPdfImageSizeH / heightHighDpi;
                            targetDpi = Math.Min(widthHighDpi, heightHighDpi);
                        }

                        pdfImages.Add(new ImageItem
                        {
                            ImagePath = imgPath,
                            PreviewPath = previewPath,
                            IsChecked = true,
                            Dpi = targetDpi,
                            DisplayName = $"PDF {pdfIndex + 1} – Seite {i + 1}"
                        });
                    }

                    pdfIndex++;
                }
            });

            fileListView.ItemsSource = pdfImages;
            busyOverlay.IsActivityRunning = false;
            busyOverlay.IsOverlayVisible = false;
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }


    private async Task LoadPDFImages()
    {
        List<ImageItem> pdfImages = [];
        busyOverlay.IsOverlayVisible = true;
        busyOverlay.IsActivityRunning = true;
        busyOverlay.BusyMessage = "PDF wird konvertiert...";

        await Task.Run(() =>
        {
            int pdfIndex = 0;

            foreach (var file in resultList)
            {
                byte[] bytearray = File.ReadAllBytes(file.FullPath);
                int pagecount = Conversion.GetPageCount(bytearray);

                if (!Directory.Exists(Settings.CacheDirectory))
                    Directory.CreateDirectory(Settings.CacheDirectory);

                for (int i = 0; i < pagecount; i++)
                {
                    string imgBaseName = $"pdf_{pdfIndex}_page_{i}";
                    string imgPath = Path.Combine(Settings.DataDirectory, Settings.CacheDirectory, imgBaseName + ".jpg");
                    string previewPath = Path.Combine(Settings.DataDirectory, Settings.CacheDirectory, "preview_" + imgBaseName + ".jpg");

                    var renderOptions = new RenderOptions()
                    {
                        AntiAliasing = PdfAntiAliasing.All,
                        Dpi = targetDpi,
                        WithAnnotations = true,
                        WithFormFill = true,
                        UseTiling = true,
                    };
                    Conversion.SaveJpeg(imgPath, bytearray, i, options: renderOptions);

                    var stream = File.OpenRead(imgPath);
                    var skBitmap = SKBitmap.Decode(stream);
                    Size _imgSize = new(skBitmap.Width, skBitmap.Height);

                    if (File.Exists(previewPath))
                        File.Delete(previewPath);
                }

                pdfIndex++;
            }
        });

        busyOverlay.IsActivityRunning = false;
        busyOverlay.IsOverlayVisible = false;
    }

    public static async Task<IEnumerable<FileResult>> PickPdfFileAsync()
    {
        try
        {
            var fileResult = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {                
                PickerTitle = "Eine oder mehrere PDF-Dateien auswählen",
                FileTypes = FilePickerFileType.Pdf // Nur PDF-Dateien anzeigen                
            });

            if (fileResult != null)
                return fileResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Auswählen der Datei: {ex.Message}");
        }
        return null; // Kein PDF ausgewählt
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        var cacheFiles = Directory.GetFiles(Settings.CacheDirectory);
        foreach (var cacheFile in cacheFiles)
        {
            File.Delete(cacheFile);
        }
        Shell.Current.GoToAsync("..");
    }

    private void OnPagesAddClicked(object sender, EventArgs e)
    {
        AddPdfImages();
    }

    private async void AddPdfImages()
    {
        await LoadPDFImages(); //generiere High-Res Images
        
        string imageDirectory = Path.Combine(Settings.DataDirectory, GlobalJson.Data.ProjectPath, GlobalJson.Data.PlanPath);
        int i = 0;

        // Überprüfen, ob Plans null ist, und es gegebenenfalls initialisieren
        GlobalJson.Data.Plans ??= [];  // Initialisiere Plans, wenn es null ist

        foreach (var item in fileListView.ItemsSource.Cast<ImageItem>())
        {
            if (item.IsChecked)
            {
                string sourceFilePath = item.ImagePath;
                string fileName = "plan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + i + ".jpg";
                string planId = "plan_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + i;
                string destinationFilePath = Path.Combine(imageDirectory, fileName);

                var stream = File.OpenRead(Path.Combine(Settings.CacheDirectory, item.ImagePath));
                var skBitmap = SKBitmap.Decode(stream);
                Size _imgSize = new(skBitmap.Width, skBitmap.Height);

                Plan plan = new()
                {
                    Name = item.DisplayName,
                    File = fileName,
                    ImageSize = _imgSize,
                    IsGrayscale = false,
                    Description = "",
                    AllowExport = true,
                };

                // Überprüfen, ob die Plans-Struktur initialisiert ist
                GlobalJson.Data.Plans ??= [];
                GlobalJson.Data.Plans[Path.GetFileNameWithoutExtension(fileName)] = plan;

                File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                i += 1;

                // fügr neue Pläne hinzu
                var newPlan = new KeyValuePair<string, Models.Plan>(planId, plan);
                LoadDataToView.AddPlan(newPlan);
            }
        }

        GlobalJson.SaveToFile();

        var cacheFiles = Directory.GetFiles(Settings.CacheDirectory);
        foreach (var cacheFile in cacheFiles)
        {
            File.Delete(cacheFile);
        }

        await Shell.Current.GoToAsync("project_details");
    }

    private void OnChangeRowsClicked(object sender, EventArgs e)
    {
        if (DynamicSpan == 1)
        {
            DynamicSpan = 0;
            btnRows.IconImageSource = new FontImageSource
            {
                FontFamily = "MaterialOutlined",
                Glyph = UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Grid_on,
                Color = Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Application.Current.Resources["Primary"]
                        : (Color)Application.Current.Resources["PrimaryDark"]
            };
            btnRows.Text = "Kacheln";
        }
        else
        {
            DynamicSpan = 1;
            btnRows.IconImageSource = new FontImageSource
            {
                FontFamily = "MaterialOutlined",
                Glyph = UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Table_rows,
                Color = Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Application.Current.Resources["Primary"]
                        : (Color)Application.Current.Resources["PrimaryDark"]
            };
            btnRows.Text = "Liste";
        }
        UpdateSpan();
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        UpdateSpan();
    }

    private void UpdateSpan()
    {
        if (DynamicSpan != 1)
        {
            double screenWidth = this.Width;
            double imageWidth = Settings.PlanPreviewSize; // Mindestbreite in Pixeln
            DynamicSpan = Math.Max(2, (int)(screenWidth / imageWidth));
        }
        OnPropertyChanged(nameof(DynamicSpan));
    }
}
