namespace NxPlx.Configuration
{
    public class Configuration
    {
#if DEBUG
        public readonly bool Production = false;
#else
        public readonly bool Production = true;
#endif
        
        public int HttpPort { get; set; }
        public int JobServerPort { get; set; }

        public string ImageFolder { get; set; }
        public string LogFolder { get; set; }
        
        public string SqlHost { get; set; }
        public string SqlMediaDatabase { get; set; }
        public string SqlUserDatabase { get; set; }
        public string SqlJobDatabase { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
        
        public string RedisHost { get; set; }
        public string RedisPassword { get; set; }
        
        
        public string ProbeToken { get; set; }
        
        public string TMDbApiKey { get; set; }
    }
}