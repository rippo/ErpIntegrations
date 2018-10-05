using System;
using Webcrm.ErpIntegrations.Configurations;
using Xunit;

namespace Webcrm.ErpIntegrations.GeneralUtilities.Test
{
    public class DateUtilitiesTester
    {
        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCestInMay()
        {
            var dateToTest = new DateTime(2018, 5, 17, 22, 3, 43);
            var result = dateToTest.FromUtcToSwedish();
            Assert.Equal(new DateTime(2018, 5, 18, 00, 3, 43), result);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCestInOct()
        {
            var dateToTest = new DateTime(2018, 10, 27, 7, 3, 43);
            var result = dateToTest.FromUtcToSwedish();
            Assert.Equal(new DateTime(2018, 10, 27, 9, 3, 43), result);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCetInFeb()
        {
            var dateToTest = new DateTime(2018, 2, 1, 2, 1, 27);
            var result = dateToTest.FromUtcToSwedish();
            Assert.Equal(new DateTime(2018, 2, 1, 3, 1, 27), result);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestUtcToSwedishCetInOct()
        {
            var dateToTest = new DateTime(2018, 10, 30, 2, 1, 27);
            var result = dateToTest.FromUtcToSwedish();
            Assert.Equal(new DateTime(2018, 10, 30, 3, 1, 27), result);
        }
    }
}