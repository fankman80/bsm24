#nullable disable

using Mopups.Pages;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace bsm24.Views;

public partial class PopupColorPicker : PopupPage
{
    TaskCompletionSource<int> _taskCompletionSource;
    public Task<int> PopupDismissedTask => _taskCompletionSource.Task;
    public int ReturnValue { get; set; }
    private readonly int LineWidth;
    public ObservableCollection<Color> Colors { get; set; }

    public PopupColorPicker(int lineWidth, string okText = "Ok")
    {
	InitializeComponent();
        okButtonText.Text = okText;
        LineWidth = lineWidth;

        Colors =
        [
            Color.FromRgb(255,0,0), Color.FromRgb(255,255,0), Color.FromRgb(0,255,255), Color.FromRgb(0,0,255), Color.FromRgb(0,255,255),
            Color.FromRgb(255,0,255), Color.FromRgb(255,0,0), Color.FromRgb(255,0,0), Color.FromRgb(255,0,0), Color.FromRgb(255,0,0),
                // ... füge hier weitere Farben hinzu
        ];
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        sliderText.Text = "Pinselgrösse: " + LineWidth.ToString();
        LineWidthSlider.Value = LineWidth;

        _taskCompletionSource = new TaskCompletionSource<int>();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult(ReturnValue);
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        ReturnValue = LineWidth;
        await MopupService.Instance.PopAsync();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        ReturnValue = (int)LineWidthSlider.Value;
        await MopupService.Instance.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        ReturnValue = LineWidth;
        await MopupService.Instance.PopAsync();
    }

    private void OnSliderValueChanged(object sender, EventArgs e)
    {
        var sliderValue = ((Slider)sender).Value;
        sliderText.Text = "Skalierung: " + Math.Round(sliderValue, 0).ToString() + "%";
    }

}
