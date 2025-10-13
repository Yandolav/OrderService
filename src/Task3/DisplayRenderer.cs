using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Task3.Options;

namespace Task3;

public class DisplayRenderer : IDisplayRenderer
{
    private readonly IOptionsMonitor<DisplayOptions> _options;
    private readonly IHttpClientFactory _http;
    private readonly ILogger<DisplayRenderer> _logger;

    public DisplayRenderer(IOptionsMonitor<DisplayOptions> options, IHttpClientFactory http, ILogger<DisplayRenderer> logger)
    {
        _options = options;
        _http = http;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        await RenderOnceAsync(ct);
        _options.OnChange(async void (opts) => { await RenderOnceAsync(ct); });
        try
        {
            await Task.Delay(Timeout.Infinite, ct);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task RenderOnceAsync(CancellationToken ct)
    {
        DisplayOptions current = _options.CurrentValue;
        AnsiConsole.Clear();

        if (current.Mode == DisplayMode.Figlet)
        {
            string text = current.Text ?? "Unknown text";
            var figlet = new FigletText(text);
            AnsiConsole.Write(figlet);
            return;
        }

        if (current.Mode == DisplayMode.Base64Image)
        {
            if (string.IsNullOrWhiteSpace(current.ImageBase64))
            {
                AnsiConsole.MarkupLine("[red]display:imageBase64 is empty[/]");
                return;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(current.ImageBase64);
                using var ms = new MemoryStream(bytes, writable: false);
                var canvas = new CanvasImage(ms);
                canvas.MaxWidth(80);
                AnsiConsole.Write(canvas);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Invalid base64 image: {Markup.Escape(ex.Message)}[/]");
            }

            return;
        }

        if (current.Mode == DisplayMode.UrlImage)
        {
            if (string.IsNullOrWhiteSpace(current.ImageUrl))
            {
                AnsiConsole.MarkupLine("[red]display:imageUrl is empty[/]");
                return;
            }

            try
            {
                HttpClient client = _http.CreateClient();
                byte[] bytes = await client.GetByteArrayAsync(current.ImageUrl, ct);
                using var ms = new MemoryStream(bytes, writable: false);
                var canvas = new CanvasImage(ms);
                canvas.MaxWidth(80);
                AnsiConsole.Write(canvas);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to load image from URL: {Markup.Escape(ex.Message)}[/]");
            }

            return;
        }

        AnsiConsole.MarkupLine($"[yellow]Unknown mode: {current.Mode}[/]");
    }
}
