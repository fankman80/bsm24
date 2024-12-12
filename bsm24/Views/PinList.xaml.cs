#nullable disable

using UraniumUI.Pages;
using bsm24.Models;

namespace bsm24.Views;

public partial class PinList : UraniumContentPage
{
    public Command<IconItem> IconTappedCommand { get; }
    public int DynamicSpan { get; set; } = 1;
    public int MinSize { get; set; } = 1;

    public PinList()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
        BindingContext = this;

        LoadPins();
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        UpdateSpan();
    }

    private void LoadPins()
    {
        List<Pin> pinItems = [];

        foreach (var plan in GlobalJson.Data.Plans)
        {
            if (GlobalJson.Data.Plans[plan.Key].Pins != null)
            {
                foreach (var pin in GlobalJson.Data.Plans[plan.Key].Pins)
                {
                    pinItems.Add(new Pin
                    {
                        PinDesc = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinDesc,
                        PinIcon = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinIcon,
                        PinName = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinName,
                        PinLocation = GlobalJson.Data.Plans[plan.Key].Pins[pin.Key].PinLocation
                    });
                }
            }
        }
        pinListView.ItemsSource = pinItems;
        pinListView.Footer = "";
    }

    private async void OnPinClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var fileName = button.AutomationId;
    }

    private async void UpdateSpan()
    {
        busyOverlay.IsVisible = true;
        activityIndicator.IsRunning = true;
        busyText.Text = "Icons werden geladen...";

        await Task.Run(() =>
        {
            OnPropertyChanged(nameof(DynamicSpan));
        });

        activityIndicator.IsRunning = false;
        busyOverlay.IsVisible = false;
    }
}
