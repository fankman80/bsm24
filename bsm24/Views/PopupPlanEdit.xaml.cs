#nullable disable

using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bsm24.Views;

public partial class PopupPlanEdit : Popup<PlanEditReturn>, INotifyPropertyChanged
{
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

        BindingContext = this;
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        await CloseAsync(new PlanEditReturn(name_entry.Text, desc_entry.Text, allow_export.IsChecked, PlanRotate));
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        await CloseAsync(new PlanEditReturn("delete", null, true, PlanRotate));
    }
    private async void OnGrayscaleClicked(object sender, EventArgs e)
    {
        await CloseAsync(new PlanEditReturn("grayscale", null, true, PlanRotate));
    }

    private void PlanRotateLeft(object sender, EventArgs e)
    {
        PlanRotate -= 90;

        if (PlanRotate < 0)
            PlanRotate = 360 + PlanRotate;
    }

    private void PlanRotateRight(object sender, EventArgs e)
    {
        PlanRotate += 90;

        if (PlanRotate > 270)
            PlanRotate = 0;
    }

    private int _planRotate = 0;
    public int PlanRotate
    {
        get => _planRotate;
        set
        {
            if (_planRotate != value)
            {
                _planRotate = value;
                OnPropertyChanged(nameof(PlanRotate));
            }
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
