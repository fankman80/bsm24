#nullable disable

using Mopups.Pages;
using Mopups.Services;

namespace bsm24.Views;

public partial class PopupAlert : PopupPage
{
    public PopupAlert(string title, string okText = "Ok")
	{
		InitializeComponent();
        titleText.Text = title;
        okButtonText.Text = okText;
    }

    private async void PopupPage_BackgroundClicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}