using System;
using RedHttpServerCore;

namespace NextPlex
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RedHttpServer(5000, "./public");
            Console.WriteLine("Hello World!");
        }
    }
}
