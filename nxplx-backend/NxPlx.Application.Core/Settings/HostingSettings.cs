using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Settings
{
    public class HostingSettings : ISettings
    {
        [Required]
        public string Origin { get; set; } = null!;

        [Required]
        public bool Secure { get; set; }

        [Required]
        public bool ApiDocumentation { get; set; }
    }
}