#nullable disable

using UraniumUI.Icons.MaterialIcons;

namespace bsm24;

public partial class LoadDataToView
{
    public async Task LoadData(FileResult path)
    {
        if (path != null && !string.IsNullOrEmpty(path.FullPath))
        {
            if (GlobalJson.Data.plans != null)
            {
                foreach (var plan in GlobalJson.Data.plans)
                {
                    string planTitle = GlobalJson.Data.plans[plan.Key].name;
                    string planId = plan.Key;

                    // Define the new page
                    var newPage = new Views.NewPage(planId)
                    {
                        Title = planTitle,
                        AutomationId = planId,
                    };

                    // Create a new FlyoutItem
                    var newFlyoutItem = new FlyoutItem
                    {
                        Title = planTitle,
                        AutomationId = planId,
                        Icon = new FontImageSource { FontFamily = "MaterialOutlined", Color = Colors.Black, Glyph = MaterialTwoTone.Layers },
                        Items = { new ShellContent { Content = newPage } }
                    };

                    // Register the route
                    Routing.RegisterRoute(planId, typeof(Views.NewPage));

                    // Add the new FlyoutItem to the AppShell
                    (Application.Current.MainPage as AppShell).Items.Add(newFlyoutItem);
                }
            }
        }
    }

    public async Task ResetApp()
    {
        // Liste für zu entfernende ShellItems erstellen
        var itemsToRemove = new List<ShellItem>();

        // Alle ShellItems durchlaufen und zu entfernende Items sammeln
        foreach (var shellitem in (Application.Current.MainPage as AppShell).Items)
        {
            if (shellitem.AutomationId != null)
            {
                itemsToRemove.Add(shellitem);
            }
        }

        // Jetzt die gesammelten Items entfernen, nachdem die Iteration abgeschlossen ist
        foreach (var shellitem in itemsToRemove)
        {
            (Application.Current.MainPage as AppShell).Items.Remove(shellitem);
        }

        // Reset Datenbank
        GlobalJson.Data.client_name = null;
        GlobalJson.Data.object_address = null;
        GlobalJson.Data.working_title = null;
        GlobalJson.Data.object_name = null;
        GlobalJson.Data.creation_date = null;
        GlobalJson.Data.project_manager = null;
        GlobalJson.Data.pdf = null;
        GlobalJson.Data.plans = null;
        GlobalJson.Data.planPath = null;
        GlobalJson.Data.imagePath = null;
        GlobalJson.Data.thumbnailPath = null;
        GlobalJson.Data.projectPath = null;
        GlobalJson.Data.jsonFile = null;
    }
}
public class FileItem
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileDate { get; set; }
    public string ImagePath { get; set; }
    public string ThumbnailPath { get; set; }
}
