using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class HostingOptions : INxplxOptions
    {
        [Required]
        public string Origin { get; set; }

        public bool Secure { get; set; } = false;
        public bool Swagger { get; set; } = false;
        public bool HangfireDashboard { get; set; } = false;
    }
}