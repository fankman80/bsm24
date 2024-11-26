#nullable disable

using Mopups.Pages;
using Mopups.Services;

namespace bsm24.Views;

public partial class PopupSettings : PopupPage
{
    TaskCompletionSource<string> _taskCompletionSource;
    public Task<string> PopupDismissedTask => _taskCompletionSource.Task;
    public string ReturnValue { get; set; }

    public PopupSettings()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _taskCompletionSource = new TaskCompletionSource<string>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult(ReturnValue);
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        ReturnValue = null;
        await MopupService.Instance.PopAsync();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        ReturnValue = null;
        await MopupService.Instance.PopAsync();
    }

    private async void OnExportSettingsClicked(object sender, EventArgs e)
    {
        // Show Settings Page
        var popup = new PopupExportSettings();
        await MopupService.Instance.PushAsync(popup);
        var result = await popup.PopupDismissedTask;
        if (result != null)
        {

        }
    }

    private void OnSelectedValueChanged(object sender, EventArgs e)
    {
        // Hole die AppShell
        var appShell = (AppShell)Application.Current.Windows[0].Page;

        // Durchlaufe die FlyoutItems und aktualisiere die Icon-Farbe
        foreach (var item in appShell.Items)
        {
            if (item is FlyoutItem flyoutItem && flyoutItem.Icon is FontImageSource fontIcon)
            {
                // Aktualisiere die Farbe des Icons basierend auf dem aktuellen Theme
                fontIcon.Color = (Color)(Application.Current.RequestedTheme == AppTheme.Dark
                                ? Application.Current.Resources["PrimaryDark"]
                                : Application.Current.Resources["Primary"]);
            }
        }
    }
}