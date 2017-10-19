using System;
using System.Collections.Generic;
using NxPlx.Models;
using RedHttpServerCore;

namespace NxPlx
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RedHttpServer(5000, "./public");
            var media = new MediaManager();

            var files = new Dictionary<string, Movie>();
            files.Add("0123456789", new Movie());
            server.Get("/mediafile/:fileid", async (req, res) =>
            {
                
            });
            Console.WriteLine("Hello World!");
        }
    }
}
