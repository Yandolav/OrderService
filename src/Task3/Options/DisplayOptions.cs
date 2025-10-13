namespace Task3;

public class DisplayOptions
{
    public DisplayMode Mode { get; set; } = DisplayMode.Figlet;

    public string? Text { get; set; }

    public string? ImageBase64 { get; set; }

    public string? ImageUrl { get; set; }
}
