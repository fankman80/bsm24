#nullable disable

using System.ComponentModel;

namespace bsm24;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = serviceProvider.GetService<AppShell>();

        // Setze die Kultur der Anwendung auf Deutsch (Deutschland)
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
        System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");

        SetTheme();

        Services.SettingsService.Instance.PropertyChanged += OnSettingsPropertyChanged;
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        if (window != null)
        {
            window.Title = "BSM 24 by EBBE";
        }

        return window;
    }

    private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SetTheme();
    }

    private void SetTheme()
    {
        UserAppTheme = Services.SettingsService.Instance?.Theme != null
                     ? Services.SettingsService.Instance.Theme.AppTheme
                     : AppTheme.Unspecified;
    }
}
