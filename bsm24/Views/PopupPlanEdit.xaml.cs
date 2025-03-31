#nullable disable

using Mopups.Pages;
using Mopups.Services;

namespace bsm24.Views;

public partial class PopupPlanEdit : PopupPage
{
    TaskCompletionSource<(string, string, bool)> _taskCompletionSource;
    public Task<(string, string, bool)> PopupDismissedTask => _taskCompletionSource.Task;
    public (string, string, bool) ReturnValue { get; set; }

    public PopupPlanEdit(string name, string desc, bool gray, bool export = true, string okText = "Ok", string cancelText = "Abbrechen")
    {
        InitializeComponent();
        okButtonText.Text = okText;
        cancelButtonText.Text = cancelText;
        name_entry.Text = name;
        desc_entry.Text = desc;
        allow_export.IsChecked = export;

        if (gray)
            grayscaleButtonText.Text = "Farben hinzufügen";
        else
            grayscaleButtonText.Text = "Farben entfernen";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _taskCompletionSource = new TaskCompletionSource<(string, string, bool)>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult(ReturnValue);
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        ReturnValue = (null, null, true);
        await MopupService.Instance.PopAsync();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        ReturnValue = (name_entry.Text, desc_entry.Text, allow_export.IsChecked);
        await MopupService.Instance.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        ReturnValue = (null, null, true);
        await MopupService.Instance.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        ReturnValue = ("delete", null, true);
        await MopupService.Instance.PopAsync();
    }
    private async void OnGrayscaleClicked(object sender, EventArgs e)
    {
        ReturnValue = ("grayscale", null, true);
        await MopupService.Instance.PopAsync();
    }
}
