using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NUnit.Framework;
using NxPlx.Application.Core;
using NxPlx.Infrastructure.Database;

namespace NxPlx.Test.UnitTests
{
    public class DataModelIntegrityTests
    {
        [Test]
        public async Task PendingModelChangesTest()
        {
            var dummyConnectionString = "Host=localhost;Database=nxplx_db;Username=postgres;Password=dev";
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql(dummyConnectionString,
                b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)).Options;
            await using var databaseContext = new DatabaseContext(databaseContextOptions, new OperationContext());

            var modelDiffer = databaseContext.GetService<IMigrationsModelDiffer>();
            var migrationsAssembly = databaseContext.GetService<IMigrationsAssembly>();

            var dependencies = databaseContext.GetService<ProviderConventionSetBuilderDependencies>();
            var relationalDependencies = databaseContext.GetService<RelationalConventionSetBuilderDependencies>();

            var typeMappingConvention = new TypeMappingConvention(dependencies);
            typeMappingConvention.ProcessModelFinalizing(((IConventionModel)migrationsAssembly.ModelSnapshot.Model).Builder, null);

            var relationalModelConvention = new RelationalModelConvention(dependencies, relationalDependencies);
            var sourceModel = relationalModelConvention.ProcessModelFinalized(migrationsAssembly.ModelSnapshot.Model);

            var finalSourceModel = ((IMutableModel)sourceModel).FinalizeModel().GetRelationalModel();
            var finalTargetModel = databaseContext.Model.GetRelationalModel();

            var differences = modelDiffer.GetDifferences(finalSourceModel, finalTargetModel);
            if (differences.Any())
            {
                Assert.True(false, $"{differences.Count} changes between migrations and model. Debug this test for more details");
            }

            Assert.Pass();
        }
    }
}