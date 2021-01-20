using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class HostingOptions : INxplxOptions
    {
        [Required]
        public string Origin { get; set; } = null!;

        [Required]
        public bool Secure { get; set; } = true;
    }
}