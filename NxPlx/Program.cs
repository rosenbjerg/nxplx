using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandbrakeCLIwrapper;
using NxPlx.Models;
using RedHttpServerCore;
using RedHttpServerCore.Request;
using RedHttpServerCore.Response;

namespace NxPlx
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RedHttpServer(5000, "./public");
            var media = new MediaManager();
            var transcoder = new TranscodingManager();

            transcoder.Enqueue(null, null);
            var files = new Dictionary<string, Movie>();
            files.Add("0123456789", new Movie());
            server.Get("/mediafile/:fileid", async (req, res) =>
            {
                
            });
            server.Get("/transcoder/status", transcoder.GetStatus);
            server.WebSocket("/transcoder/statusupdates", transcoder.GetStatus);
            server.Post("/transcoder/enqueue", transcoder.Enqueue);
            server.Post("/transcoder/stop", transcoder.Stop);

            server.Start();
            Console.ReadLine();
        }
        
    }

    class TranscodeJob
    {
        public string InputFile { get; set; }
        public string OutputDirectory { get; set; }
        public HandbrakeCliConfigBuilder Configuration { get; set; }
    }

    static class TranscodingConfigurations
    {
        public static HandbrakeCliConfigBuilder Web720p30fps = new HandbrakeCliConfigBuilder();
    }

    class TranscodingManager
    {
        
        private readonly HandbrakeCli _handbrakeCli;
        private readonly Queue<TranscodeJob> _transcodeQueue = new Queue<TranscodeJob>();
        private bool _started = false;
        private readonly List<WebSocketDialog> _statusSubscribers = new List<WebSocketDialog>();
        private bool _sendingUpdates;

        public TranscodingManager()
        {
            _handbrakeCli = new HandbrakeCli("./HandbrakeCLI.exe");
            _handbrakeCli.TranscodingCompleted += HandbrakeCliOnTranscodingCompleted;
            _handbrakeCli.TranscodingStarted += HandbrakeCliOnTranscodingStarted;
            _handbrakeCli.TranscodingError += HandbrakeCliOnTranscodingError;
        }

        private bool ConvertNext()
        {
            if (!_transcodeQueue.TryDequeue(out var job))
                return false;
            _handbrakeCli.Convert(job.Configuration, job.InputFile, job.OutputDirectory);
            return true;
        }

        private void HandbrakeCliOnTranscodingStarted(object sender, HandbrakeTranscodingEventArgs ev)
        {
            SendToUpdateSubscribers("{\"Status\":\"Transcode job started\",\"StatusCode\":1,\"File\":\""+ev.InputFilename+"\"}");
        }

        private void HandbrakeCliOnTranscodingCompleted(object sender, HandbrakeTranscodingEventArgs ev)
        {
            SendToUpdateSubscribers("{\"Status\":\"Transcode job completed\",\"StatusCode\":2,\"File\":\"" + ev.InputFilename + "\"}");
            ConvertNext();
        }

        private void HandbrakeCliOnTranscodingError(object sender, HandbrakeTranscodingEventArgs ev)
        {
            SendToUpdateSubscribers("{\"Status\":\"Transcode job error\",\"StatusCode\":3,\"File\":\"" + ev.InputFilename + "\"}");
            ConvertNext();
        }

        public async Task GetStatus(RRequest req, RResponse res)
        {
            await res.SendJson(_handbrakeCli.Status);
        }

        private void SendToUpdateSubscribers(string msg)
        {
            _statusSubscribers.ForEach(sub => sub.SendText(msg));
        }

        public void GetStatus(RRequest req, WebSocketDialog wsd)
        {
            _statusSubscribers.Add(wsd);
            wsd.OnClosed += delegate { _statusSubscribers.Remove(wsd); };
            if (!_sendingUpdates)
                SendStatus();
        }

        private async void SendStatus()
        {
            _sendingUpdates = true;
            await Task.Delay(200);
            while (_statusSubscribers.Count != 0)
            {
                var status = ServiceStack.Text.JsonSerializer.SerializeToString(_handbrakeCli.Status);
                SendToUpdateSubscribers(status);
                await Task.Delay(1500);
            }
            _sendingUpdates = false;
        }

        public async Task Enqueue(RRequest req, RResponse res)
        {
            _transcodeQueue.Enqueue(new TranscodeJob
            {
                InputFile = "C:\\Users\\Malte\\Desktop\\Top Gear - 12x03 - .mkv",
                OutputDirectory = "C:\\Users\\Malte\\Desktop",
                Configuration = TranscodingConfigurations.Web720p30fps
            });
            if (!_handbrakeCli.Status.Converting)
                ConvertNext();
            //await res.SendString("OK");
        }

        public Task Stop(RRequest req, RResponse res)
        {
            throw new NotImplementedException();
        }
    }
    
}
