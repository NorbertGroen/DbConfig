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
                ["AosWet:WW:Applicatie"] = "Naam van de WW applicatie",
                ["AosWet:WW:Onderwerp"] = "Naam van het WW onderwerp",
                ["AosWet:WW:SubOnderwerpnderwerp"] = "Naam van het WW subonderwerp",
                ["AosWet:IOW:Applicatie"] = "Naam van de IOW applicatie",
                ["AosWet:IOW:Onderwerp"] = "Naam van het IOW onderwerp",
                ["AosWet:IOW:SubOnderwerpnderwerp"] = "Naam van het IOW subonderwerp",
            };
            context.Settings.AddRange(
             aosWettenSetting
             .Select(kvp => new Settings(kvp.Key, kvp.Value))
             .ToArray());

            context.SaveChanges();

            return widgetOptionSetting;
        }
    }
}