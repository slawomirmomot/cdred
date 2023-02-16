using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

using Test2;


internal partial class Program
{
    private static void Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();

        try
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .Build();

              

              using var servicesProvider = new ServiceCollection()
             
             .AddTransient<CustomerFunctionaTest>() // Runner is the custom class
                                                    //using var channel = GrpcChannel.ForAddress("https://localhost:7212");
                                                    //var client = new Greeter.GreeterClient(channel);

             .AddLogging(loggingBuilder =>
             {
                 // configure Logging with NLog
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                 loggingBuilder.AddNLog(config);
             }).
             AddTransient<CustomerFunctionaTest>()
             .BuildServiceProvider();
            //.// Runner is the custom class

            var runner = servicesProvider.GetRequiredService<CustomerFunctionaTest>();
            runner.DoAction("Action1");

            //WaitHandle.WaitAll(endOfWorkEvents);
        
        }
        catch (Exception ex)
        {
            // NLog: catch any exception and log it.
            logger.Error(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
    }
}



