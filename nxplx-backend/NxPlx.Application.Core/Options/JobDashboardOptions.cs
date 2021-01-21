using NxPlx.Application.Core.ValidationFilters;

namespace NxPlx.Application.Core.Options
{
    public class JobDashboardOptions : INxplxOptions
    {
        public bool Enabled { get; set; } = false;
        
        [RequiredIf(nameof(Enabled), true)]
        public string Username { get; set; } = null!;
        [RequiredIf(nameof(Enabled), true)]
        public string Password { get; set; } = null!;
    }
}