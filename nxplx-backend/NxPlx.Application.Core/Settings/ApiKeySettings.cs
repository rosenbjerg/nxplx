using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Settings
{
    public class ApiKeySettings : ISettings
    {
        [Required]
        public string Probe { get; set; } = null!;
        
        [Required]
        public string TMDB { get; set; } = null!;
    }
}