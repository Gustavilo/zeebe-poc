using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ZeebePOC.Service
{
  /// <summary>
  /// 
  /// </summary>
  public class Startup
  {
    #region :: Properties ::

    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get; }

    #endregion

    #region :: Constructor ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration) => Configuration = configuration;

    #endregion

    #region :: Methods ::

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "ZeebePOC Service",
          Version = "v1",
          Contact = new OpenApiContact
          {
            Name = "Gustavo Alfaro Aké",
            Email = "gustavo.alfaro@conekta.com",
          },
        });

        var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
        c.IncludeXmlComments(xmlFile);
      });

      services.AddLogging();
    }

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

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZeebePOC Service");
      });

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

    #endregion
  }
}
