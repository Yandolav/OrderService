namespace Task1;

public class ConfigClientOptions
{
    public string BaseAddress { get; set; } = "http://localhost:8080";

    public int PageSize { get; set; } = 50;

    public string Implementation { get; set; } = "Refit";
}