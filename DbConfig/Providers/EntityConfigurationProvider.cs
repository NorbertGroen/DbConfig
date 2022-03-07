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

            var AosCodes = new AosCodes
            {
                [Wet.WW] =
                new AosCode
                {
                    Applicatie = "applicatie WW",
                    Onderwerp = "Dit is het WW onderwerp",
                    Subonderwerp = "En het WW subonderwerp"
                },
                [Wet.ZW] =
                new AosCode
                {
                    Applicatie = "applicatie ZW",
                    Onderwerp = "Dit is het ZW onderwerp",
                    Subonderwerp = "En het ZW subonderwerp"
                }
            };
            var aosWettenSetting = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                [$"Aos:{Wet.WWF}:Applicatie"] = "WWF applicatie",
                [$"Aos:{Wet.WWF}:Onderwerp"] = "WWF onderwerp",
                [$"Aos:{Wet.WWF}:Subonderwerp"] = "WWF subonderwerp",
                [$"Aos:{Wet.IOW}:Applicatie"] = "IOW applicatie",
                [$"Aos:{Wet.IOW}:Onderwerp"] = "IOW onderwerp",
                [$"Aos:{Wet.IOW}:Subonderwerp"] = "IOW subonderwerp",
            };
            context.Settings.AddRange(
             aosWettenSetting
             .Select(kvp => new Settings(kvp.Key, kvp.Value))
             .ToArray());
            context.Settings.AddRange(
                GetSettingsArray(AosCodes)
            );

            context.SaveChanges();

            return widgetOptionSetting.Merge(aosWettenSetting).Merge(GetDictionary(AosCodes));
        }

        private static Dictionary<string, string> GetDictionary(AosCodes AosCodes)
        {
            return AosCodes.Select(d => new Dictionary<string, string>
            {
                [$"Aos:{d.Key}:Applicatie"]= d.Value.Applicatie,
                [$"Aos:{d.Key}:Onderwerp"]= d.Value.Onderwerp,
                [$"Aos:{d.Key}:Subonderwerp"]= d.Value.Subonderwerp
            }).SelectMany(i => i).ToDictionary(a => a.Key, a => a.Value, StringComparer.OrdinalIgnoreCase);
        }


        private static Settings[] GetSettingsArray(AosCodes AosCodes)
        {
            return AosCodes.Select(d => new Settings[]
            {
                new Settings(id: $"Aos:{d.Key}:Applicatie", value: d.Value.Applicatie),
                new Settings(id: $"Aos:{d.Key}:Onderwerp", value: d.Value.Onderwerp),
                new Settings(id: $"Aos:{d.Key}:Subonderwerp", value: d.Value.Subonderwerp)
            }).SelectMany(i => i).ToArray();
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
