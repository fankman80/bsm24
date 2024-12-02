using System.Collections.ObjectModel;
using UraniumUI.Pages;
using FFImageLoading.Maui;

#nullable disable

namespace bsm24.Views;

[QueryProperty(nameof(PlanId), "planId")]
[QueryProperty(nameof(PinId), "pinId")]

public partial class IconGallery : UraniumContentPage, IQueryAttributable
{
    public string PlanId { get; set; }
    public string PinId { get; set; }
    public ObservableCollection<string> Images { get; set; }
    public int DynamicSpan { get; set; } = 5; // Standardwert

    public IconGallery()
    {
        InitializeComponent();
        UpdateSpan();
        SizeChanged += OnSizeChanged;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("planId", out object value1))
        {
            PlanId = value1 as string;
        }
        if (query.TryGetValue("pinId", out object value2))
        {
            PinId = value2 as string;
        }

        MyView_Load();

        BindingContext = this;
    }

    private void MyView_Load()
    {
        Images = new ObservableCollection<string>(Settings.PinData.Select(item => item.fileName));
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        UpdateSpan();
    }

    private void UpdateSpan()
    {
        double screenWidth = this.Width;
        double iconWidth = 64; // Mindestbreite der Icons in Pixeln
        DynamicSpan = Math.Max(5, (int)(screenWidth / iconWidth));
        OnPropertyChanged(nameof(DynamicSpan));
    }

    private async void OnImageTapped(object sender, EventArgs e)
    {
        var tappedImage = sender as CachedImage;
        var fileName = ((FileImageSource)tappedImage.Source).File;

        GlobalJson.Data.Plans[PlanId].Pins[PinId].PinIcon = fileName;

        // suche Pin-Daten
        GlobalJson.Data.Plans[PlanId].Pins[PinId].PinTxt = Settings.PinData.FirstOrDefault(item => item.fileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).imageName;
        GlobalJson.Data.Plans[PlanId].Pins[PinId].Anchor = Settings.PinData.FirstOrDefault(item => item.fileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).anchor;
        GlobalJson.Data.Plans[PlanId].Pins[PinId].Size = Settings.PinData.FirstOrDefault(item => item.fileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).size;
        GlobalJson.Data.Plans[PlanId].Pins[PinId].IsLockRotate = Settings.PinData.FirstOrDefault(item => item.fileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).isLockRotate;

        // save data to file
        GlobalJson.SaveToFile();

        await Shell.Current.GoToAsync($"..?planId={PlanId}&pinId={PinId}&pinIcon={fileName}");
    }
}
