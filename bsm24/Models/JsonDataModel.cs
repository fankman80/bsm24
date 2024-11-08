#nullable disable

public class JsonDataModel
{
    public string client_name { get; set; }
    public string object_address { get; set; }
    public string working_title { get; set; }
    public string object_name { get; set; }
    public string creation_date { get; set; }
    public string project_manager { get; set; }
    public Pdf pdf { get; set; }
    public Dictionary<string, Plan> plans { get; set; }
    public string planPath { get; set; }
    public string imagePath { get; set; }
    public string imageOverlayPath { get; set; }
    public string thumbnailPath { get; set; }
    public string projectPath { get; set; }
    public string jsonFile { get; set; }
}

public class Pdf
{
    public string file { get; set; }
    public Point pos { get; set; }
    public int rotation { get; set; }
}

public class Plan
{
    public string name { get; set; }
    public string file { get; set; }
    public Size imageSize { get; set; }
    public Dictionary<string, Pin> pins { get; set; }
}

public class Pin
{
    public Point pos { get; set; }
    public Point anchor { get; set; }
    public Size size { get; set; }
    public bool isLocked { get; set; }
    public bool isLockRotate { get; set; }
    public string infoTxt { get; set; }
    public string pinTxt { get; set; }
    public string pinIcon { get; set; }
    public Dictionary<string, Image> images { get; set; }
}

public class Image
{
    public string file { get; set; }
    public string overlayFile { get; set; }
    public bool isChecked { get; set; }
}

public class Position
{
    public float X { get; set; }
    public float Y { get; set; }
}