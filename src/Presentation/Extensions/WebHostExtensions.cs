using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Presentation.Options;

namespace Presentation.Extensions;

public static class WebHostExtensions
{
    public static void ConfigureGrpcHost(this ConfigureWebHostBuilder webHost, IConfiguration configuration)
    {
        var opts = new GrpcServerOptions();
        configuration.GetSection("GrpcServer").Bind(opts);

        if (string.IsNullOrWhiteSpace(opts.Url)) throw new InvalidOperationException("GrpcServer:Url is not configured.");

        webHost.UseKestrel();
        webHost.ConfigureKestrel(k =>
        {
            k.ConfigureEndpointDefaults(o => o.Protocols = HttpProtocols.Http2);
        });
        webHost.UseUrls(opts.Url);
    }
}