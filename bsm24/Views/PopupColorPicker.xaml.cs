#nullable disable

using Mopups.Pages;
using Mopups.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bsm24.Views;

public partial class PopupColorPicker : PopupPage, INotifyPropertyChanged
{
    TaskCompletionSource<(Color, int)> _taskCompletionSource;
    public ObservableCollection<ColorBoxItem> ColorsList { get; set; }
    public Task<(Color, int)> PopupDismissedTask => _taskCompletionSource.Task;
    public (Color, int) ReturnValue { get; set; }
    public bool LineWidthVisibility { get; set; }

    public PopupColorPicker(int lineWidth, Color selectedColor, bool lineWidthVisibility = true, string okText = "Ok")
    {
	    InitializeComponent();
        okButtonText.Text = okText;
        LineWidthVisibility = lineWidthVisibility;
        LineWidth = lineWidth;
        ColorsList = new ObservableCollection<ColorBoxItem>(
                    Settings.ColorData.Select(c => new ColorBoxItem
                    { BackgroundColor = c }));

        // Prüfen, ob selectedColor in der Liste vorkommt
        var matchingItem = ColorsList.FirstOrDefault(c => c.BackgroundColor.ToHex() == selectedColor.ToHex());
        if (matchingItem != null)
        {
            SelectedColor = matchingItem.BackgroundColor;
            ColorListPicker.SelectedItem = matchingItem;
        }            
        else if (ColorsList.Count > 0)
        {
            SelectedColor = ColorsList[0].BackgroundColor;
            ColorListPicker.SelectedItem = ColorsList[0];
        }            
        
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _taskCompletionSource = new TaskCompletionSource<(Color, int)>();
    }

    private void OnColorTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border backgroundcolor && backgroundcolor.BindingContext is ColorBoxItem item)
        {
            RedValue = (int)(item.BackgroundColor.Red * 255);
            GreenValue = (int)(item.BackgroundColor.Green * 255);
            BlueValue = (int)(item.BackgroundColor.Blue * 255);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _taskCompletionSource.SetResult((SelectedColor, LineWidth));
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        ReturnValue = (SelectedColor, LineWidth);
        await MopupService.Instance.PopAsync();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        ReturnValue = (SelectedColor, LineWidth);
        await MopupService.Instance.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        ReturnValue = (SelectedColor, LineWidth);
        await MopupService.Instance.PopAsync();
    }

    private int redValue;
    public int RedValue
    {
        get => redValue;
        set
        {
            if (redValue != value)
            {
                redValue = value;
                OnPropertyChanged();
                UpdateSelectedColor();
            }
        }
    }

    private int greenValue;
    public int GreenValue
    {
        get => greenValue;
        set
        {
            if (greenValue != value)
            {
                greenValue = value;
                OnPropertyChanged();
                UpdateSelectedColor();
            }
        }
    }

    private int blueValue;
    public int BlueValue
    {
        get => blueValue;
        set
        {
            if (blueValue != value)
            {
                blueValue = value;
                OnPropertyChanged();
                UpdateSelectedColor();
            }
        }
    }

    private Color selectedColor;
    public Color SelectedColor
    {
        get => selectedColor;
        set
        {
            if (selectedColor != value)
            {
                selectedColor = value;
                OnPropertyChanged();
            }
        }
    }

    private int lineWidth;
    public int LineWidth
    {
        get => lineWidth;
        set
        {
            if (lineWidth != value)
            {
                lineWidth = value;
                OnPropertyChanged();
            }
        }
    }

    private void UpdateSelectedColor()
    {
        SelectedColor = Color.FromRgb(RedValue, GreenValue, BlueValue);
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class ColorBoxItem
{
    public Color BackgroundColor { get; set; }
}

