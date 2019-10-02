using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NxPlx.Services.Database
{
    public class DatabaseContextManager
    {
        public void Register(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<MediaContext>()
                .AddDbContext<UserContext>()
                .BuildServiceProvider();
        }

        public async Task Initialize()
        {
            using (var context = new MediaContext())
            {
                await context.Database.EnsureCreatedAsync();
            }
            using (var context = new UserContext())
            {
                await context.Database.EnsureCreatedAsync();
            }
        }
    }
}