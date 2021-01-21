using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NxPlx.Test.UnitTests
{
    public class AutoMapperTests
    {
        [Test]
        public void VerifyDtoProfile()
        {
            var bench = UnitTestBench.CreateAndBuild();
            var mapper = bench.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}