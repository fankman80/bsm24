
#nullable disable

using System.Reflection;

namespace bsm24;

public class Helper
{
    public static void FlyoutItemState(string itemRoute, bool isVisible)
    {
        if ((Application.Current.Windows[0].Page as AppShell).Items
        .SelectMany(item => item.Items) // Alle FlyoutItem/ShellSections durchsuchen
        .SelectMany(section => section.Items) // Alle ShellContent-Items durchsuchen
        .FirstOrDefault(content => content.Route == itemRoute) is ShellContent shellContent)
            shellContent.FlyoutItemIsVisible = isVisible;
    }

    public static void AddMenuItemAtPosition(string title, string glyph, string methodName)
    {
        // Erstelle das neue MenuItem
        var newMenuItem = new MenuItem
        {
            Text = title,
            //AutomationId = "999",
            IconImageSource = new FontImageSource
            {
                FontFamily = "MaterialOutlined",
                Glyph = glyph,
                Color = Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Application.Current.Resources["PrimaryDark"]
                        : (Color)Application.Current.Resources["Primary"]
            }
        };

        if (Application.Current.Windows[0].Page is AppShell appShell)
        {
            // Verwende Reflection, um die Methode aus appShell zu finden und aufzurufen
            var methodInfo = appShell.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (methodInfo != null)
                newMenuItem.Clicked += (s, e) => methodInfo.Invoke(appShell, [s, e]);
            else
                Console.WriteLine($"Methode '{methodName}' wurde nicht gefunden.");
        }

        // Füge das neue MenuItem hinzu
        if (Shell.Current.Items is IList<ShellItem> shellItems)
            shellItems.Add(newMenuItem);
    }
}