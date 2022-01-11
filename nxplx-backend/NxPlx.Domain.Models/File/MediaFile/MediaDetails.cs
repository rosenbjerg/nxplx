namespace NxPlx.Domain.Models.File
{
    public class MediaDetails
    {
        public string VideoCodec { get; set; }
        public float VideoFrameRate { get; set; }
        public long VideoBitrate { get; set; }
        public int VideoBitDepth { get; set; }
        public int VideoHeight { get; set; }
        public int VideoWidth { get; set; }
        public string AudioCodec { get; set; }
        public long AudioBitrate { get; set; }
        public string AudioChannelLayout { get; set; }
        public int AudioStreamIndex { get; set; }
        public float Duration { get; set; }
        public string VideoAspectRatio { get; set; }
    }
}