namespace NxPlx.Models.File
{
    public class FFMpegProbeDetails : EntityBase
    {
        public string VideoCodec { get; set; }
        
        public int VideoFrameRate { get; set; }
        
        public int VideoBitrate { get; set; }
        
        public int VideoBitDepth { get; set; }
        
        public int VideoHeight { get; set; }
        
        public int VideoWidth { get; set; }
        
        
        public string AudioCodec { get; set; }
        
        public int AudioBitrate { get; set; }
                
        public int AudioChannels { get; set; }
        
        public int AudioSamplingRateHz { get; set; }
        
        
        public bool WebOptimized { get; set; }
    }
}