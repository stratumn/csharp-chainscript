using Xunit;
using System.Collections.Generic;
using Stratumn.Chainscript.ChainscriptTest.TestCases;

namespace Stratumn.Chainscript.ChainscriptTest
{
    public class ChainscriptTest
    {
        public static IEnumerable<object[]> GetTestCases()
        {
            yield return new object[] { new SimpleSegmentTest() };
            yield return new object[] { new ReferencesTest() };
            yield return new object[] { new EvidencesTest() };
            yield return new object[] { new SignaturesTest() };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void TestChainscript(ITestCase testCase)
        {
            Assert.True(testCase.Validate(testCase.Generate()));
        }
    }
}