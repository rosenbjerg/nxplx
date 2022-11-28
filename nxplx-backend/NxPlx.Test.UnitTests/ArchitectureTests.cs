using System;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using NxPlx.ApplicationHost.Api;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace NxPlx.Test.UnitTests
{
    public class ArchitectureTests
    {
        [Test]
        public void VerifyDependencies()
        {
            var hostBuilder = Program.PrepareWebHostBuilder(Array.Empty<string>());
            using var host = hostBuilder.Build();
            Assert.NotNull(host);
        }
        
        private static readonly Architecture Architecture =
            new ArchLoader().LoadAssemblies(new []
                {
                    typeof(Abstractions.IOperationContext).Assembly,
                    typeof(Application.Core.JobQueueNames).Assembly,
                    typeof(Application.Events.SubOperationScopeCommand).Assembly,
                    typeof(Application.Mapping.MappingAssemblyMarker).Assembly,
                    typeof(Application.Models.IDto).Assembly,
                    typeof(Application.Services.AssemblyMarker).Assembly,
                    typeof(ApplicationHost.Api.Program).Assembly,
                    typeof(Domain.Events.MediaOverviewQuery).Assembly,
                    typeof(Domain.Models.ITrackedEntity).Assembly,
                    typeof(Domain.Services.AssemblyMarker).Assembly,
                    typeof(Infrastructure.Broadcasting.ConnectionHub).Assembly,
                    typeof(Infrastructure.Database.DatabaseContext).Assembly,
                    typeof(Infrastructure.Events.ServiceCollectionExtensions).Assembly,
                    typeof(Integrations.TMDb.TMDbApi).Assembly,
                    typeof(Services.Index.FileIndexer).Assembly,
                })
                .Build();
        
        private readonly IObjectProvider<Class> _controllerClasses =
            Classes().That().AreAssignableTo("ControllerBase").As("Controllers");
        
        private readonly IObjectProvider<IType> _domainLayer =
            Types().That().ResideInNamespace("NxPlx.Domain").As("Domain Layer");
        
        private readonly IObjectProvider<IType> _applicationLayer =
            Types().That().ResideInNamespace("NxPlx.Application").And().DoNotResideInNamespace("NxPlx.ApplicationHost").As("Application Layer");
        
        private readonly IObjectProvider<IType> _integrationLayer =
            Types().That().ResideInNamespace("NxPlx.Integrations").As("Integration Layer");
        
        private readonly IObjectProvider<IType> _infrastructureLayer =
            Types().That().ResideInNamespace("NxPlx.Infrastructure").As("Infrastructure Layer");
        
        private readonly IObjectProvider<IType> _applicationHostLayer =
            Types().That().ResideInNamespace("NxPlx.ApplicationHost").As("ApplicationHost Layer");
        
        [Test]
        public void VerifyControllerAuthentication()
        {
            var rule = 
                Classes().That().Are(_controllerClasses)
                    .And().DoNotHaveName("StreamController")
                    .And().DoNotHaveName("AuthenticationController")
                    .Should().HaveAnyAttributes("SessionAuthentication");
            
            rule.Check(Architecture);
        }
        
        // [Test]
        // public void VerifyDomainLayerUsage()
        // {
        //     var rule =
        //         Types().That().Are(_domainLayer)
        //             .Should().NotDependOnAny(_applicationLayer)
        //             .AndShould().NotDependOnAny(_integrationLayer)
        //             .AndShould().NotDependOnAny(_applicationHostLayer);
        //     
        //     rule.AssertPassed(Architecture);
        // }
        
        [Test]
        public void VerifyApplicationLayerUsage()
        {
            var rule =
                Types().That().Are(_applicationLayer)
                    .Should().NotDependOnAny(_applicationHostLayer);
            
            rule.AssertPassed(Architecture);
        }
    }
}