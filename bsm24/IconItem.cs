using SkiaSharp;

namespace bsm24;

public class IconItem(string fileName, string displayName, Point anchorPoint, Size iconSize, bool isRotationLocked, SKColor pinColor, double iconScale)
{
    public string FileName { get; set; } = fileName;
    public string DisplayName { get; set; } = displayName;
    public Point AnchorPoint { get; set; } = anchorPoint;
    public Size IconSize { get; set; } = iconSize;
    public bool IsRotationLocked { get; set; } = isRotationLocked;
    public bool IsCustomPin { get; set; } = false;
    public SKColor PinColor { get; set; } = pinColor;
    public double IconScale { get; set; } = iconScale;
}