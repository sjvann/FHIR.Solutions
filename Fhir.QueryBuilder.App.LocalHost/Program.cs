using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;

var contentRoot = AppContext.BaseDirectory;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = Path.Combine(contentRoot, "wwwroot"),
});

var urls = builder.Configuration["LocalHost:Urls"] ?? "http://127.0.0.1:8797";
builder.WebHost.UseUrls(urls);

var app = builder.Build();

if (!Directory.Exists(app.Environment.WebRootPath))
{
    Console.Error.WriteLine(
        $"Missing web root: '{app.Environment.WebRootPath}'. " +
        "Install must include a 'wwwroot' folder next to this executable.");
    return 1;
}

var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");

app.UseDefaultFiles(defaultFilesOptions);

// Blazor WASM 會請求 _framework 下的 .dat（ICU）、.wasm、.webcil 等；預設 StaticFiles
// 對「未知副檔名」直接 404，瀏覽器會卡在載入畫面。
var staticFileOptions = new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
};
var contentTypes = new FileExtensionContentTypeProvider();
contentTypes.Mappings[".dat"] = "application/octet-stream";
contentTypes.Mappings[".webcil"] = "application/octet-stream";
contentTypes.Mappings[".blat"] = "application/octet-stream";
contentTypes.Mappings[".wasm"] = "application/wasm";
contentTypes.Mappings[".webmanifest"] = "application/manifest+json";
staticFileOptions.ContentTypeProvider = contentTypes;

app.UseStaticFiles(staticFileOptions);

// 僅允許 loopback，供同一台電腦上的 Blazor UI 結束 Kestrel（關閉「整個後端」）。
app.MapPost(
    "/api/app/shutdown",
    (HttpContext http, IHostApplicationLifetime lifetime) =>
    {
        if (http.Connection.RemoteIpAddress is null ||
            !IPAddress.IsLoopback(http.Connection.RemoteIpAddress))
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        http.Response.OnCompleted(() =>
        {
            lifetime.StopApplication();
            return Task.CompletedTask;
        });
        return Results.Text("OK", "text/plain");
    });

app.MapFallbackToFile("index.html");

var launchBrowser = app.Configuration.GetValue("LocalHost:LaunchBrowser", true);
var openUrl = NormalizeBrowserUrl(urls);

if (launchBrowser && openUrl is not null)
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = openUrl,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Could not launch browser: {ex.Message}");
        }
    });
}

Console.WriteLine($"FHIR Query Builder — open in browser: {openUrl ?? urls}");
Console.WriteLine("Press Ctrl+C to stop, or close this console window.");

app.Run();

return 0;

static string? NormalizeBrowserUrl(string urlsConfig)
{
    var first = urlsConfig.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .FirstOrDefault();
    if (string.IsNullOrEmpty(first))
    {
        return null;
    }

    return first.Replace("http://*", "http://127.0.0.1", StringComparison.OrdinalIgnoreCase)
        .Replace("http://0.0.0.0", "http://127.0.0.1", StringComparison.OrdinalIgnoreCase)
        .Replace("http://+", "http://127.0.0.1", StringComparison.OrdinalIgnoreCase);
}
