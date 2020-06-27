using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class ApiKeyOptions : INxplxOptions
    {
        [Required]
        public string ProbeKey { get; set; }
        [Required]
        public string TmdbKey { get; set; }
    }
}