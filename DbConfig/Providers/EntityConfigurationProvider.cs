using System;
using System.Collections.Generic;
using System.Linq;
using DbConfig.Models;
using DbConfig.Providers;
using Microsoft.Extensions.Configuration;

namespace CustomProvider.Example.Providers
{
    public class EntityConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connectionString;

        public EntityConfigurationProvider(string connectionString) =>
            _connectionString = connectionString;

        public override void Load()
        {
            using var dbContext = new EntityConfigurationContext(_connectionString);

            dbContext.Database.EnsureCreated();

            Data = dbContext.Settings.Any()
                ? dbContext.Settings.ToDictionary(c => c.Id, c => c.Value)
                : CreateAndSaveDefaultValues(dbContext);
        }

        static IDictionary<string, string> CreateAndSaveDefaultValues(
            EntityConfigurationContext context)
        {
            var widgetOptionSetting = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["WidgetOptions:EndpointId"] = "b3da3c4c-9c4e-4411-bc4d-609e2dcc5c67",
                ["WidgetOptions:DisplayLabel"] = "Widgets Incorporated, LLC.",
                ["WidgetOptions:WidgetRoute"] = "api/widgets"
            };


            context.Settings.AddRange(
                widgetOptionSetting.Select(kvp => new Settings(kvp.Key, kvp.Value))
                        .ToArray());

            var aosWettenSetting = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                [$"Aos:{Wet.WW}:Applicatie"] = "WW applicatie",
                [$"Aos:{Wet.WW}:Onderwerp"] = "WW onderwerp",
                [$"Aos:{Wet.WW}:Subonderwerp"] = "WW subonderwerp",
                [$"Aos:{Wet.IOW}:Applicatie"] = "IOW applicatie",
                [$"Aos:{Wet.IOW}:Onderwerp"] = "IOW onderwerp",
                [$"Aos:{Wet.IOW}:Subonderwerp"] = "IOW subonderwerp",
            };
            context.Settings.AddRange(
             aosWettenSetting
             .Select(kvp => new Settings(kvp.Key, kvp.Value))
             .ToArray());

            context.SaveChanges();

            return widgetOptionSetting.Merge(aosWettenSetting);
        }
    }

    public static class ExtensionMethods 
    { 
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB)
            where TValue : class
        {
            return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
        }
    }
}
