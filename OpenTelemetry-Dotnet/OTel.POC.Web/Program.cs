using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace OTel.POC.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Activity.DefaultIdFormat = ActivityIdFormat.W3C;
      var listener = new ActivityListener
      {
        ShouldListenTo = _ => true,
        ActivityStopped = activity =>
        {
          foreach (var (key, value) in activity.Baggage)
          {
            activity.AddTag(key, value);
          }
        }

      };
      ActivitySource.AddActivityListener(listener);
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((context, builder) =>
            {

              builder.AddConsole();

              builder.AddOpenTelemetry(options =>
              {
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.IncludeFormattedMessage = true;

                options.AddConsoleExporter();
              });
            });
  }
}
