#nullable disable
using bsm24.Views;

namespace bsm24;

public partial class LoadDataToView
{

    public static void LoadData(FileResult path)
    {
        if (path == null || string.IsNullOrEmpty(path.FullPath))
            return;

        if (GlobalJson.Data.Plans == null)
            return;

        if (Application.Current.Windows[0].Page is not AppShell appShell)
            return;

        foreach (var plan in GlobalJson.Data.Plans)
        {
            string planId = plan.Key;
            string planTitle = plan.Value.Name;

            // Neue Plan-Seite mit Übergabe der ID erstellen
            var newPage = new Views.NewPage(planId)
            {
                Title = planTitle,
                AutomationId = planId,
                PlanId = planId,
            };

            // ShellContent erzeugen und mit eindeutiger Route versehen
            var shellContent = new ShellContent
            {
                Content = newPage,
                Route = planId, // Wichtig: wird für GoToAsync("//planId") verwendet
                Title = planTitle,
                Icon = "icon_placeholder.png" // Optional: Standardicon oder aus IconGlyph generieren
            };

            // Seite zur Shell dynamisch hinzufügen
            appShell.Items.Add(shellContent);

            // PlanItem für ein Flyout- oder Menü-Item hinzufügen
            appShell.PlanItems.Add(new PlanItem(GlobalJson.Data.Plans[plan.Key])
            {
                Title = planTitle,
                PlanId = planId,
                IconGlyph = UraniumUI.Icons.MaterialSymbols.MaterialOutlined.Layers,
                PlanRoute = planId
            });
        }
    }

    public static void ResetData()
    {
        (Application.Current.Windows[0].Page as AppShell).PlanItems.Clear();

        // Reset Datenbank
        GlobalJson.Data.Client_name = null;
        GlobalJson.Data.Object_address = null;
        GlobalJson.Data.Working_title = null;
        GlobalJson.Data.Object_name = null;
        GlobalJson.Data.Creation_date = DateTime.Now;
        GlobalJson.Data.Project_manager = null;
        GlobalJson.Data.PlanPdf = null;
        GlobalJson.Data.Plans = null;
        GlobalJson.Data.PlanPath = null;
        GlobalJson.Data.ImagePath = null;
        GlobalJson.Data.ThumbnailPath = null;
        GlobalJson.Data.CustomPinsPath = null;
        GlobalJson.Data.ProjectPath = null;
        GlobalJson.Data.JsonFile = null;
    }
}
