using System;
using System.IO;
using Newtonsoft.Json;

namespace NxPlx.Configuration
{
    public static class ConfigurationService
    {
#if DEBUG
        public static readonly Configuration Current = LoadFromFile();
#else
        public static readonly Configuration Current = LoadFromEnvironment();
#endif

        private static Configuration LoadFromEnvironment()
        {
            return new Configuration
            {
                HttpPort = 5353,
                JobServerPort = 5354,
                ImageFolder = "/app/data/images",
                LogFolder = "/app/data/logs",
                SqlJobDatabase = "nxplx_job_db",
                SqlMediaDatabase = "nxplx_media_db",
                SqlUserDatabase = "nxplx_user_db",
                RedisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "nxplx-cache",
                SqlHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "nxplx-database",
                SqlUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres",
                RedisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD"),
                SqlPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
                ProbeToken = Environment.GetEnvironmentVariable("NXPLX_PROBE_TOKEN"),
                TMDbApiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY")
            };
        }
        private static Configuration LoadFromFile()
        {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
        }
    }
}