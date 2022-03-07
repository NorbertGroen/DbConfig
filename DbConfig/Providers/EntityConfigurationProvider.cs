using DbConfig.Extensions;
using DbConfig.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbConfig.Providers
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
            context.Settings.AddRange(aosCodes.GetSettings());

            context.SaveChanges();

            return aosWettenSetting.Merge(aosCodes.GetDictionary());
        }
    }
}
