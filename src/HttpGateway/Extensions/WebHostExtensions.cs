using HttpGateway.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace HttpGateway.Extensions;

public static class WebHostExtensions
{
    public static void ConfigureHttpHost(this ConfigureWebHostBuilder webHost, IConfiguration configuration)
    {
        var opts = new HttpServerOptions();
        configuration.GetSection("HttpServer").Bind(opts);

        if (string.IsNullOrWhiteSpace(opts.Url)) throw new InvalidOperationException("HttpServer:Url is not configured.");

        webHost.UseKestrel(k =>
        {
            k.ConfigureEndpointDefaults(o => o.Protocols = HttpProtocols.Http1);
        });
        webHost.UseUrls(opts.Url);
    }
}