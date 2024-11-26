using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;

namespace bsm24.Views;

public partial class VoidPage : ContentPage
{
	public VoidPage()
	{
		InitializeComponent();

        RunCodeInBackground();
    }

    private async void RunCodeInBackground()
    {
        string outputPath = Path.Combine(FileSystem.AppDataDirectory, GlobalJson.Data.ProjectPath, GlobalJson.Data.ProjectPath + ".docx");
        await ExportReport.DocX("template.docx", outputPath);

        CancellationToken cancellationToken = new();
        var saveStream = File.Open(outputPath, FileMode.Open);
        var fileSaveResult = await FileSaver.Default.SaveAsync(GlobalJson.Data.ProjectPath + ".docx", saveStream, cancellationToken);
        if (fileSaveResult.IsSuccessful)
            await Toast.Make($"Bericht wurde gespeichert: {fileSaveResult.FilePath}").Show(cancellationToken);
        else
            await Toast.Make($"Bericht wurde nicht gespeichert: {fileSaveResult.Exception.Message}").Show(cancellationToken);
        saveStream.Close();
        File.Delete(outputPath);
    }
}