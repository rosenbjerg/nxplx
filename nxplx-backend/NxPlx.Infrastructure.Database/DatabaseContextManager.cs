using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace NxPlx.Services.Database
{
    public class DatabaseContextManager
    {
        public void Register(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<MediaContext>()
                .BuildServiceProvider();
            
        }

        public void Initialize()
        {
            using (var context = new MediaContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}