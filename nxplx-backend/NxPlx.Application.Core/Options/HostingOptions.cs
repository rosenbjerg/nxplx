using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class HostingOptions : INxplxOptions
    {
        [Required]
        public string Origin { get; set; }

        [Required]
        public bool Secure { get; set; } = true;
        public bool ApiDocumentation { get; set; } = false;
        public bool HangfireDashboard { get; set; } = false;
    }
}