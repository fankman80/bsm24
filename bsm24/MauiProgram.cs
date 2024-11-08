using Camera.MAUI;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using Mopups.Hosting;
using MR.Gestures;
using UraniumUI;
using CommunityToolkit.Maui.Storage;


#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace bsm24;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
	    var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCameraView()
            .UseMauiCommunityToolkit()
            .UseUraniumUI()
            .UseFFImageLoading()
            .ConfigureMRGestures()
            .ConfigureMopups()
            .UseFFImageLoading()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialIconFonts();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Registriere die AppShell
        builder.Services.AddSingleton<AppShell>();

        // Registriere den FileSaver
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);


        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
        {
            if (view is Microsoft.Maui.Controls.Entry)
            {
#if ANDROID
                handler.PlatformView.Background=null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth= 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            }

        });

        return builder.Build();
	}
}