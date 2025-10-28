#nullable disable

using SkiaSharp;
using SnapDoc.Models;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SnapDoc
{
    public class IconItem(string fileName, string displayName, Point anchorPoint, Size iconSize, bool isRotationLocked, SKColor pinColor, double iconScale, string category)
    {
        public string FileName { get; set; } = fileName;
        public string DisplayName { get; set; } = displayName;
        public Point AnchorPoint { get; set; } = anchorPoint;
        public Size IconSize { get; set; } = iconSize;
        public bool IsRotationLocked { get; set; } = isRotationLocked;
        public bool IsCustomPin { get; set; } = false;
        public SKColor PinColor { get; set; } = pinColor;
        public double IconScale { get; set; } = iconScale;
        public string Category { get; set; } = category;
    }

    public class FileItem
    {
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required DateTime FileDate { get; set; }
        public string DateTimeDisplay => FileDate.ToString("dd. MMMM yyyy' / 'HH:mm", new CultureInfo("de-DE"));
        public required string ImagePath { get; set; }
        public required string ThumbnailPath { get; set; }
    }

    public partial class PinItem : ObservableObject
    {
        public required string PinDesc { get; set; }
        public required string PinIcon { get; set; }
        public required string PinName { get; set; }
        public required string PinLocation { get; set; }
        public required string OnPlanId { get; set; }
        public required string OnPlanName { get; set; }
        public required string SelfId { get; set; }
        public required DateTime Time { get; set; }

        [ObservableProperty] private bool _allowExport;
    }

    public class ColorPickerReturn(string colorHex, int width)
    {
        public string PenColorHex { get; set; } = colorHex;
        public int PenWidth { get; set; } = width;
    }

    public class PlanEditReturn(string nameEntry, string descEntry, bool allowExport, int planRotate, string planColor)
    {
        public string NameEntry { get; set; } = nameEntry;
        public string DescEntry { get; set; } = descEntry;
        public bool AllowExport { get; set; } = allowExport;
        public int PlanRotate { get; set; } = planRotate;
        public string PlanColor { get; set; } = planColor;
    }

    public class PriorityItem
    {
        public required string Key { get; set; }
        public required string Color { get; set; }
    }

    public class MapViewItem
    {
        public required string Desc { get; set; }
        public required string Id { get; set; }
    }

    public partial class PlanItem : ObservableObject
    {
        private readonly Plan _plan; // direkte Referenz auf das zugrundeliegende Modell

        public PlanItem(Plan plan)
        {
            _plan = plan ?? throw new ArgumentNullException(nameof(plan));

            _plan.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Plan.AllowExport))
                    OnPropertyChanged(nameof(AllowExport));
                else if (e.PropertyName == nameof(Plan.PlanColor))
                    OnPropertyChanged(nameof(PlanColor));
                else if (e.PropertyName == nameof(Plan.PinCount))
                    OnPropertyChanged(nameof(PinCount));
            };
        }

        public string PlanId { get; set; } = string.Empty;
        public string PlanRoute { get; set; } = string.Empty;

        public bool AllowExport
        {
            get => _plan.AllowExport;
            set
            {
                if (_plan.AllowExport != value)
                {
                    _plan.AllowExport = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PlanColor
        {
            get => _plan.PlanColor;
            set
            {
                if (_plan.PlanColor != value)
                {
                    _plan.PlanColor = value;
                    OnPropertyChanged(nameof(PlanColor));
                }
            }
        }

        public int PinCount
        {
            get => _plan.PinCount;
            set
            {
                if (_plan.PinCount != value)
                {
                    _plan.PinCount = value;
                    OnPropertyChanged(nameof(PinCount));
                }
            }
        }

        [ObservableProperty] private string _title;

        [ObservableProperty] private bool _isSelected;
    }

    public partial class FotoItem : ObservableObject
    {
        public string ImagePath { get; set; }
        public DateTime DateTime { get; set; }

        [ObservableProperty] private bool _allowExport;
    }

    public class PdfItem
    {
        public string ImagePath { get; set; }
        public string PreviewPath { get; set; }
        public string PdfPath { get; set; }
        public bool IsChecked { get; set; }
        public int Dpi { get; set; }
        public string DisplayName { get; set; }
        public string ImageName { get; set; }
        public int PdfPage { get; set; }
    }
}
