using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Predictly_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Read Configuration from appSettings    
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //Initialize Logger    
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
            try
            {
                Log.Information("Application Starting.##############3");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.######333");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            //CreateHostBuilder(args).Build().Run(); 
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}
