using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;
using NUnit.Framework;

namespace NxPlx.Test.UnitTests
{
    public static class ArchUnitExtensions
    {
        public static void AssertPassed(this TypesShouldConjunction provider, Architecture architecture)
        {
            Assert.Multiple(() =>
            {
                foreach (var evaluationResult in provider.Evaluate(architecture))
                {
                    Assert.IsTrue(evaluationResult.Passed, evaluationResult.Description);
                }
            });
        }
        public static void AssertPassed(this ClassesShouldConjunction provider, Architecture architecture)
        {
            Assert.Multiple(() =>
            {
                foreach (var evaluationResult in provider.Evaluate(architecture))
                {
                    Assert.IsTrue(evaluationResult.Passed, evaluationResult.Description);
                }
            });
        }
    }
}