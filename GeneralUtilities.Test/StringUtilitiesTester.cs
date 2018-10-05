using Webcrm.ErpIntegrations.Configurations;
using Xunit;

namespace Webcrm.ErpIntegrations.GeneralUtilities.Test
{
    public class StringUtilitiesTester
    {
        [Theory]
        [Trait(Traits.Execution, Traits.Automatic)]
        [InlineData("abc", "abc", true)]
        [InlineData("abc", "cde", false)]
        [InlineData("abc", "ABC", false)]
        [InlineData("", null, true)]
        [InlineData(null, "", true)]
        [InlineData(null, null, true)]
        [InlineData(" ", "", false)]
        public void TestAreEquivalent(string stringA, string stringB, bool expectedResult)
        {
            bool actualResult = StringUtilities.AreEquivalent(stringA, stringB);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [Trait(Traits.Execution, Traits.Automatic)]
        [InlineData("\r\n", "", "", "", "")]
        [InlineData("\r\n", "1", "", "", "1")]
        [InlineData("\r\n", "", "2", "", "2")]
        [InlineData("\r\n", "1", "", "3", "1\r\n3")]
        [InlineData("\r\n", "1", "2", "", "1\r\n2")]
        [InlineData("\r\n", "1", "2", "3", "1\r\n2\r\n3")]
        [InlineData("\r\n", null, null, null, "")]
        public void TestJoinDefined(string separator, string string1, string string2, string string3, string expectedConcatenatedString)
        {
            string actualConcatenatedString = StringUtilities.JoinDefined(separator, string1, string2, string3);
            Assert.Equal(expectedConcatenatedString, actualConcatenatedString);
        }
    }
}