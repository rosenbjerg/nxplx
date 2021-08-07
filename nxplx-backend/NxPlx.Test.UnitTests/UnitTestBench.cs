using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NxPlx.Application.Core;
using NxPlx.Application.Mapping;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Test.UnitTests
{
    public class UnitTestBench
    {
        private static readonly InMemoryDatabaseRoot DatabaseRoot = new InMemoryDatabaseRoot();

        public static IServiceCollection Create()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(typeof(MappingAssemblyMarker));
            serviceCollection.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("test_db", DatabaseRoot));
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddScoped(typeof(OperationContext), _ => new OperationContext());
            return serviceCollection;
        }

        public static IServiceProvider CreateAndBuild() => Create().BuildServiceProvider();
    }
}