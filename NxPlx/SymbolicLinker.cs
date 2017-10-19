using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NxPlx
{
    static class SymbolicLinker
    {
        private static readonly int _os;

        static SymbolicLinker()
        {
            _os = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 1
                : RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 2
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 3 
                        : 0;
        }

        private static void CheckInput(string source, string link)
        {
            if (!File.Exists(source))
                throw new Exception("Source file does not exists");
            if (File.Exists(link))
                throw new Exception("There already exists a file at the link location");
        }

        public static void CreateSymlink(string source, string link)
        {
            source = Path.GetFullPath(source);
            link = Path.GetFullPath(link);
            CheckInput(source, link);
            switch (_os)
            {
                case 1:
                case 3:
                    CreateSymLinkLinux(source, link);
                    break;
                case 2:
                    CreateSymLinkWindows(source, link);
                    break;
                default:
                    throw new Exception("Unrecognized OS");
            }
        }

        private static void CreateSymLinkLinux(string source, string link)
        {
            var p = Process.Start("ln", $"-s {source} {link}");
            p.WaitForExit(750);
        }
        private static void CreateSymLinkWindows(string source, string link)
        {
            var p = Process.Start("mklink", $"{link} {source}");
            p.WaitForExit(750);
        }
    }
}