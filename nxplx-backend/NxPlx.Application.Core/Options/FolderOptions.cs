using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class FolderOptions : INxplxOptions
    {
        [Required]
        public string Images { get; set; } = null!;

        public string? TempFiles { get; set; }
    }
}