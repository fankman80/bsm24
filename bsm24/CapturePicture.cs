#nullable disable

using SkiaSharp;

namespace bsm24;

public partial class CapturePicture
{
    public CapturePicture(string filepath, string thumbnailPath = null)
    {
        _ = CapturePicture.Capture(filepath, thumbnailPath);
    }

    public static async Task<FileResult> Capture(string filepath, string thumbnailPath=null, string customFilename=null)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            try
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    string originalFilePath = photo.FullPath;
                    string resultPath = null;
                    string filename;
                    if (customFilename != null)
                    {
                        filename = customFilename;
                    }
                    else
                    {
                        filename = "IMG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + Path.GetExtension(originalFilePath);
                    }

                    if (filepath != null)
                    {
                        var type = photo.ContentType;
                        string newFilePath = Path.Combine(FileSystem.AppDataDirectory, filepath, filename);
                        var originalStream = File.OpenRead(originalFilePath);
                        var newStream = File.Create(newFilePath);
                        await originalStream.CopyToAsync(newStream);
                        newStream.Close();
                        originalStream.Close();
                        resultPath = newFilePath;
                    }

                    if (thumbnailPath != null)
                    {
                        var originalStream2 = File.OpenRead(originalFilePath);
                        var skBitmap = SKBitmap.Decode(originalStream2);
                        string thumbFilePath = Path.Combine(FileSystem.AppDataDirectory, thumbnailPath, filename);

                        // Zielgröße festlegen (keine Kante kürzer als 150 Pixel)
                        int minSize = Settings.thumbSize;

                        // Berechne den Skalierungsfaktor basierend auf der kürzeren Seite
                        float scale = minSize / (float)Math.Min(skBitmap.Width, skBitmap.Height);

                        // Berechne die neue Breite und Höhe unter Beibehaltung des Seitenverhältnisses
                        int targetWidth = (int)(skBitmap.Width * scale);
                        int targetHeight = (int)(skBitmap.Height * scale);

                        // Erstelle eine neue Bitmap mit den verkleinerten Abmessungen
                        var resizedBitmap = new SKBitmap(targetWidth, targetHeight);
                        skBitmap.ScalePixels(resizedBitmap, SKSamplingOptions.Default);

                        // Speichere das verkleinerte Bild als JPEG
                        var image = SKImage.FromBitmap(resizedBitmap);
                        var data = image.Encode(SKEncodedImageFormat.Jpeg, 90); // 90 = Qualität
                        var newStream2 = File.Create(thumbFilePath);
                        data.SaveTo(newStream2);
                        newStream2.Close();
                        originalStream2.Close();
                    }

                    if (File.Exists(originalFilePath)) //lösche das Originalfoto
                        File.Delete(originalFilePath);

                    return new FileResult(resultPath);
                }
                else
                    { return null; }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                Console.WriteLine($"Fehler beim Aufnehmen oder Umbenennen des Fotos: {ex.Message}");
                return null;
            }
        }
        else
            { return null; }
    }
}
