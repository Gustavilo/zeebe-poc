using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ZeebePOC.Service
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
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      var localConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();

      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(localConfiguration)
        .Enrich.WithProperty("AppId", "ZeebePOC.Service")
        .CreateLogger();

      try
      {
        CreateHostBuilder(args, localConfiguration)
          .Build()
          .Run();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Host terminated unexpectedly");
      }
      finally
      {
        Log.CloseAndFlush();
      }
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
          webBuilder
            .UseConfiguration(localConfiguration)
            .UseSerilog()
            .UseStartup<Startup>();
        });

    #endregion
  }
}
