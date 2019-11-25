using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using NxPlx.Models.File;

namespace Nxplx.Integrations.FFMpeg
{
    public class FFProbe
    {
        public static FFMpegProbeDetails Analyse(string file)
        {
            const string entries = "stream=index,avg_frame_rate,codec_name,display_aspect_ratio,width,height,duration,codec_type," +
                                   "bit_rate,bits_per_raw_sample,channel_layout";
            var arguments = $"-v error -show_entries {entries} -print_format json \"{file}\"";

            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false, 
                    RedirectStandardOutput = true, 
                    FileName = "ffprobe",
                    Arguments = arguments
                }
            };

            process.Start();
            var output = process?.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var parsedOutput = JsonSerializer.Deserialize<FFProbeOutput>(output);

            var videoStream = parsedOutput?.streams?.FirstOrDefault(stream => stream.codec_type == "video");
            var audioStream = parsedOutput?.streams?.FirstOrDefault(stream => stream.codec_type == "audio");

            if (videoStream == null || audioStream == null)
            {
                return default;
            }
            
            return new FFMpegProbeDetails
            {
                Duration = float.Parse(videoStream.duration),
                AudioStreamIndex = audioStream.index,
                AudioBitrate = int.Parse(audioStream.bit_rate),
                AudioChannelLayout = audioStream.channel_layout,
                AudioCodec = audioStream.codec_name,
                
                VideoBitrate = int.Parse(videoStream.bit_rate),
                VideoCodec = videoStream.codec_name,
                VideoAspectRatio = videoStream.display_aspect_ratio,
                VideoHeight = videoStream.height,
                VideoWidth = videoStream.width,
                VideoBitDepth = int.Parse(videoStream.bits_per_raw_sample),
                VideoFrameRate = ParseFramerate(videoStream.avg_frame_rate)
            };
        }

        private static float ParseFramerate(string framerate)
        {
            var split = framerate.Split('/');
            return float.Parse(split[0]) / float.Parse(split[1]);
        }
    }
    class Stream
    {
        public string display_aspect_ratio { get; set; }
        public string avg_frame_rate { get; set; }
        public int index { get; set; }
        public string codec_name { get; set; }
        public string codec_long_name { get; set; }
        public string codec_type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string duration { get; set; }
        public string bit_rate { get; set; }
        public string bits_per_raw_sample { get; set; }
        public string channel_layout { get; set; }
    }

    class FFProbeOutput
    {
        public List<Stream> streams { get; set; }
    }
}