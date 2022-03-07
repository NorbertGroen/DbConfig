using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var aosCodes = new AosCodes
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
                [$"AosCode:{Wet.WWF}:Applicatie"] = "WWF applicatie",
                [$"AosCode:{Wet.WWF}:Onderwerp"] = "WWF onderwerp",
                [$"AosCode:{Wet.WWF}:Subonderwerp"] = "WWF subonderwerp",
                [$"AosCode:{Wet.IOW}:Applicatie"] = "IOW applicatie",
                [$"AosCode:{Wet.IOW}:Onderwerp"] = "IOW onderwerp",
                [$"AosCode:{Wet.IOW}:Subonderwerp"] = "IOW subonderwerp",
            };
            context.Settings.AddRange(
             aosWettenSetting
             .Select(kvp => new Settings(kvp.Key, kvp.Value))
             .ToArray());
            context.Settings.AddRange(
                GetSettings(aosCodes)
            );

            context.SaveChanges();

            return aosWettenSetting.Merge(GetDictionary(aosCodes));
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseDictionary<T, Y>(Dictionary<T, Y> dictionary)
        {
            return dictionary.Select(element => element.Value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               .ToDictionary(prop => $"{typeof(Y).Name}:{element.Key}:{prop.Name}"
                               , prop => prop.GetValue(element.Value).ToString()
                               , null)).SelectMany(i => i);
        }
        private static Dictionary<string, string> GetDictionary<T, Y>(Dictionary<T, Y> dictionary)
        {
            return ParseDictionary(dictionary).ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        private static Settings[] GetSettings<T, Y>(Dictionary<T, Y> dictionary)
        {
            return ParseDictionary(dictionary).Select(kvp => new Settings(id: kvp.Key, value: kvp.Value)).ToArray();
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
