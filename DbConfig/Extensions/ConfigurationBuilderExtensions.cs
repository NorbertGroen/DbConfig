using DbConfig.Providers;
using Microsoft.Extensions.Configuration;

namespace DbConfig.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEntityConfiguration(
            this IConfigurationBuilder builder)
        {
            var tempConfig = builder.Build();
            var connectionString =
                tempConfig.GetConnectionString("DefaultConnectionString");

            return builder.Add(new EntityConfigurationSource(connectionString));
        }
    }
}