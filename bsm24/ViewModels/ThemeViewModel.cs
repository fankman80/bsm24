using Codeuctivity.OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bsm24.ViewModels;

public class ThemeViewModel : INotifyPropertyChanged
{
    private List<string> _themes;
    private string _selectedTheme;
    private List<string> _darkMode;
    private string _selectedDarkMode;

    public event PropertyChangedEventHandler PropertyChanged;

    public List<string> Themes
    {
        get => _themes;
        set
        {
            _themes = value;
            OnPropertyChanged(nameof(Themes));
        }
    }


    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value)
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));

                // Logik für das Anwenden der Farben basierend auf der Auswahl
                switch (_selectedTheme)
                {
                    case "Lachs":
                        // Setze die Primary-Farben
                        App.Current.Resources["Primary"] = Color.FromArgb("#9c4e38");
                        App.Current.Resources["PrimaryDark"] = Color.FromArgb("#c9a59b");
                        App.Current.Resources["PrimaryDarkText"] = Color.FromArgb("#ffffff");
                        break;

                    case "Gras":
                        // Setze die Secondary-Farben
                        App.Current.Resources["Primary"] = Color.FromArgb("#73b572");
                        App.Current.Resources["PrimaryDark"] = Color.FromArgb("#32a852");
                        App.Current.Resources["PrimaryDarkText"] = Color.FromArgb("#ffffff");
                        break;

                    case "Ozean":
                        // Setze die Secondary-Farben
                        App.Current.Resources["Primary"] = Color.FromArgb("#7286b5");
                        App.Current.Resources["PrimaryDark"] = Color.FromArgb("#32a852");
                        App.Current.Resources["PrimaryDarkText"] = Color.FromArgb("#ffffff");
                        break;

                    case "Feuer":
                        // Setze die Secondary-Farben
                        App.Current.Resources["Primary"] = Color.FromArgb("#eb873b");
                        App.Current.Resources["PrimaryDark"] = Color.FromArgb("#32a852");
                        App.Current.Resources["PrimaryDarkText"] = Color.FromArgb("#ffffff");
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public List<string> DarkMode
    {
        get => _darkMode;
        set
        {
            _darkMode = value;
            OnPropertyChanged(nameof(DarkMode));
        }
    }

    public string SelectedDarkMode
    {
        get => _selectedDarkMode;
        set
        {
            if (_selectedDarkMode != value)
            {
                _selectedDarkMode = value;
                OnPropertyChanged(nameof(SelectedDarkMode));

                // Logik für das Anwenden der Farben basierend auf der Auswahl
                switch (_selectedDarkMode)
                {
                    case "Light":
                        App.Current.UserAppTheme = AppTheme.Light; // Setze auf helles Theme
                        break;
                    case "Dark":
                        App.Current.UserAppTheme = AppTheme.Dark; // Setze auf dunkles Theme
                        break;
                    case "System Default":
                        App.Current.UserAppTheme = AppTheme.Unspecified; // Verwende das systemweite Theme
                        break;
                }
            }
        }
    }

    public ThemeViewModel()
    {
        Themes = new List<string> { "Lachs", "Gras", "Ozean", "Feuer" };
        DarkMode = new List<string> { "Light", "Dark", "System Default" };
        SelectedTheme = Themes[0]; // Standardauswahl
        SelectedDarkMode = DarkMode[0]; // Standardauswahl
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
