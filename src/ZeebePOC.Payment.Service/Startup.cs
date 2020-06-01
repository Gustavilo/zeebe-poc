using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZeebePOC.Payment.Service.Services;

namespace ZeebePOC.Payment.Service
{
  /// <summary>
  /// 
  /// </summary>
  public class Startup
  {
    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services) => services.AddGrpc();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<PaymentServiceContext>();

        endpoints.MapGet("/", async context =>
        {
          await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. ");
        });
      });
    }

    #endregion
  }
}
