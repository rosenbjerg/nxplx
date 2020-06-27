using System.ComponentModel.DataAnnotations;

namespace NxPlx.Application.Core.Options
{
    public class ConnectionStrings : INxplxOptions
    {
        [Required]
        public string Redis { get; set; }
        [Required]
        public string Pgsql { get; set; }
        [Required]
        public string HangfirePgsql { get; set; }
    }
}