using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ZeebePOC.Payment.Service
{
  /// <summary>
  /// 
  /// </summary>
  public class Program
  {
    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;

      var localConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(localConfiguration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Environment", environment)
        .Enrich.WithProperty("AppId", "gRPC PaymentService")
        .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss zzz}|{AppId}|{Level}|{TraceIdentifier}|{SourceContext}|{ActionName}{NewLine}{Message}{NewLine}{Exception}")
        .CreateLogger();

      CreateHostBuilder(args, localConfiguration).Build().Run();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <param name="localConfiguration"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration localConfiguration) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.ConfigureKestrel(options =>
          {
            options.ListenLocalhost(6060, o =>
            {
              o.Protocols = HttpProtocols.Http2;
            });
          });

          webBuilder
          .UseConfiguration(localConfiguration)
          .UseStartup<Startup>();
        });

    #endregion
  }
}
