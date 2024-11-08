using FFImageLoading.Maui;
using Mopups.Services;
using System.Collections.ObjectModel;
using UraniumUI.Pages;

#nullable disable

namespace bsm24.Views;

[QueryProperty(nameof(PlanId), "planId")]
[QueryProperty(nameof(PinId), "pinId")]
[QueryProperty(nameof(PinIcon), "pinIcon")]

public partial class SetPin : UraniumContentPage, IQueryAttributable
{
    public ObservableCollection<string> Images { get; set; }
    public string PlanId { get; set; }
    public string PinId { get; set; }
    public string PinIcon { get; set; }
    public int DynamicSpan { get; set; } = 3; // Standardwert
    public int DynamicSize { get; set; }

    public SetPin()
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
        if (query.TryGetValue("pinIcon", out object value3))
        {
            PinIcon = value3 as string;
            PinImage.Source = PinIcon;
        }

        MyView_Load();
        BindingContext = this;
    }

    private void MyView_Load()
    {
        Images ??= []; // Initialisiere Images, falls es null ist
        Images.Clear(); // lösche Einträge in Images

        // lese neue Bilder ein
        foreach (var img in GlobalJson.Data.plans[PlanId].pins[PinId].images)
        {
            string imgPath = GlobalJson.Data.plans[PlanId].pins[PinId].images[img.Key].file;
            Images.Add(Path.Combine(FileSystem.AppDataDirectory, GlobalJson.Data.thumbnailPath, imgPath));
        }

        ImageGallery.ItemsSource = null; // Temporär die ItemsSource auf null setzen
        ImageGallery.ItemsSource = Images; // Dann wieder auf die Collection setzen

        // read data
        PinTxt.Text = GlobalJson.Data.plans[PlanId].pins[PinId].pinTxt;
        PinInfo.Text = GlobalJson.Data.plans[PlanId].pins[PinId].infoTxt;
        PinImage.Source = GlobalJson.Data.plans[PlanId].pins[PinId].pinIcon;
        LockSwitch.IsToggled = GlobalJson.Data.plans[PlanId].pins[PinId].isLocked;
        LockRotate.IsToggled = GlobalJson.Data.plans[PlanId].pins[PinId].isLockRotate;
    }

    private async void OnImageTapped(object sender, EventArgs e)
    {
        var tappedImage = sender as CachedImage;
        var filePath = ((FileImageSource)tappedImage.Source).File;
        var fileName = new FileResult(filePath).FileName;

        await Shell.Current.GoToAsync($"imageview?imgSource={fileName}&planId={PlanId}&pinId={PinId}&pinIcon={PinIcon}");
    }

    private async void OnDeleteClick(object sender, EventArgs e)
    {
        var popup = new PopupDualResponse("Really delete this Pin?");
        await MopupService.Instance.PushAsync(popup);
        var result = await popup.PopupDismissedTask;
        if (result != null)
        {
            await Shell.Current.GoToAsync($"..?pinDelete={PinId}");
        }
    }

    private async void OnPinSelectClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"icongallery?planId={PlanId}&pinId={PinId}");
    }

    private async void OnOkayClick(object sender, EventArgs e)
    {
        // write data
        GlobalJson.Data.plans[PlanId].pins[PinId].anchor = Settings.pinData.FirstOrDefault(item => item.fileName.Equals(PinIcon, StringComparison.OrdinalIgnoreCase)).anchor;
        GlobalJson.Data.plans[PlanId].pins[PinId].size = Settings.pinData.FirstOrDefault(item => item.fileName.Equals(PinIcon, StringComparison.OrdinalIgnoreCase)).size;
        GlobalJson.Data.plans[PlanId].pins[PinId].pinTxt = PinTxt.Text;
        GlobalJson.Data.plans[PlanId].pins[PinId].infoTxt = PinInfo.Text;
        GlobalJson.Data.plans[PlanId].pins[PinId].isLocked = LockSwitch.IsToggled;
        GlobalJson.Data.plans[PlanId].pins[PinId].isLockRotate = LockRotate.IsToggled;

        // save data to file
        GlobalJson.SaveToFile();

        await Shell.Current.GoToAsync($"..?pinUpdate={PinId}");
    }

    private async void TakePhoto(object sender, EventArgs e)
    {
        FileResult path = await CapturePicture.Capture(GlobalJson.Data.imagePath, GlobalJson.Data.thumbnailPath);

        Images.Add(path.FullPath);

        Image newImageData = new()
        {
            isChecked = false,
            file = path.FileName
        };

        // Neues Image hinzufügen
        var pin = GlobalJson.Data.plans[PlanId].pins[PinId];
        pin.images[path.FileName] = newImageData;

        // save data to file
        GlobalJson.SaveToFile();
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        UpdateSpan();
    }

    private void UpdateSpan()
    {
        double screenWidth = this.Width;
        double imageWidth = Settings.thumbSize; // Mindestbreite in Pixeln
        DynamicSpan = Math.Max(3, (int)(screenWidth / imageWidth));
        DynamicSize = (int)(screenWidth / DynamicSpan);
        OnPropertyChanged(nameof(DynamicSpan));
        OnPropertyChanged(nameof(DynamicSize));
    }
}

public class SquareView : ContentView
{
    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        await Task.Yield();
        HeightRequest = Width;
    }
}

