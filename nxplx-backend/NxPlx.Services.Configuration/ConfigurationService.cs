using System.IO;
using Newtonsoft.Json;

namespace NxPlx.Configuration
{
    public static class ConfigurationService
    {
        public static readonly Configuration Current = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
    }
}