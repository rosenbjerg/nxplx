using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Settings
{
    public class FolderSettings : ISettings
    {
        [Required]
        public string Images { get; set; } = null!;

        [Required]
        public string Logs { get; set; } = null!;
    }
}