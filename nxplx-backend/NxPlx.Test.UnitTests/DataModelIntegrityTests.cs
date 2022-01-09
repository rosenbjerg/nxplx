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
            var dummyConnectionString = "Host=localhost;Database=postgres;Username=postgres;Password=postgres";
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(dummyConnectionString, b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName))
                .Options;
            await using var databaseContext = new DatabaseContext(databaseContextOptions, new OperationContext());
            
            var migrationsAssembly = databaseContext.GetService<IMigrationsAssembly>();

            if (migrationsAssembly.ModelSnapshot != null) {
                var snapshotModel = migrationsAssembly.ModelSnapshot?.Model;

                if (snapshotModel is IMutableModel mutableModel) {
                    snapshotModel = mutableModel.FinalizeModel();
                }

                snapshotModel = databaseContext.GetService<IModelRuntimeInitializer>().Initialize(snapshotModel!);
                var differences = databaseContext.GetService<IMigrationsModelDiffer>().GetDifferences(
                    snapshotModel.GetRelationalModel(),
                    databaseContext.GetService<IDesignTimeModel>().Model.GetRelationalModel());

                if (differences.Any())
                {
                    Assert.Fail($"{differences.Count} changes between migrations and model. Debug this test for more details");
                }
            
                Assert.Pass();
            }
            else
            {
                Assert.Fail("No snapshot of the model found: no migrations created yet, or incorrect MigrationsAssembly specified");
            }
        }
    }
}