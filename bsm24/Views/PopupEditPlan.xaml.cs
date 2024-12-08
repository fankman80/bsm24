#nullable disable

using Mopups.Pages;
using Mopups.Services;
using RadioButton = UraniumUI.Material.Controls.RadioButton;

namespace bsm24.Views;

public partial class PopupEditPlan : PopupPage
{

TaskCompletionSource<string> _taskCompletionSource;
public Task<string> PopupDismissedTask => _taskCompletionSource.Task;
public string ReturnValue { get; set; }

    public PopupEditPlan(string inputTxt = "")
    {
	InitializeComponent();
        ReturnValue = inputTxt;
        plan_rename.Text = inputTxt;
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
        ReturnValue = plan_rename.Text;
        await MopupService.Instance.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        ReturnValue = "Delete";
        await MopupService.Instance.PopAsync();
    }
}
