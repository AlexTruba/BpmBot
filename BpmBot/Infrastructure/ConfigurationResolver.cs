using Microsoft.Extensions.Configuration;
using System.IO;

namespace BpmBot.Infrastructure
{
    public class ConfigurationResolver
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigurationResolver()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }
        public string GetValue(string key)
        {
            return _configuration[key];
        }
    }
}
