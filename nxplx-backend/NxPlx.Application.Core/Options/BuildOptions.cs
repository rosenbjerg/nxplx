using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class BuildOptions : INxplxOptions
    {
        public string Version { get; set; } = "dev";
    }
}