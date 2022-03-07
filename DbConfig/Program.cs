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
                    services.Configure<AosCodes>(
                        context.Configuration.GetSection("AosCode"));
                })
                .Build();
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
    }
}
