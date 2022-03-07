using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CustomProvider.Example;
using DbConfig.Extensions.Configuration;
using DbConfig.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DbConfig
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.Sources.Clear();
                    configuration.AddEntityConfiguration();
                    configuration.AddInMemoryCollection();
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<WidgetOptions>(
                        context.Configuration.GetSection("WidgetOptions"));

                    services.Configure<AosCodes>(
                        context.Configuration.GetSection("Aos"));
                })
                .Build();
            TestWidgetOptions(host);
            TestAosOptions(host);

            await host.RunAsync();
        }

        private static void TestAosOptions(IHost host)
        {
            var options = host.Services.GetRequiredService<IOptions<AosCodes>>().Value;
            Console.WriteLine($"WW applicatie={options[Wet.WW].Applicatie}");
            Console.WriteLine($"WW onderwerp={options[Wet.WW].Onderwerp}");
            Console.WriteLine($"WW subonderwerp={options[Wet.WW].Subonderwerp}");

        }

        private static void TestWidgetOptions(IHost host)
        {
            var options = host.Services.GetRequiredService<IOptions<WidgetOptions>>().Value;
            Console.WriteLine($"DisplayLabel={options.DisplayLabel}");
            Console.WriteLine($"EndpointId={options.EndpointId}");
            Console.WriteLine($"WidgetRoute={options.WidgetRoute}");
        }
    }
}
// Sample output:
//    WidgetRoute=api/widgets
//    EndpointId=b3da3c4c-9c4e-4411-bc4d-609e2dcc5c67
//    DisplayLabel=Widgets Incorporated, LLC.