using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class ApiKeyOptions : INxplxOptions
    {
        public string ProbeKey { get; set; } = "dev";
        [Required]
        public string TmdbKey { get; set; } = null!;
    }
}