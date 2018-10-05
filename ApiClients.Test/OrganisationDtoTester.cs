using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Configurations.Models;
using Xunit;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class OrganisationDtoTester
    {
        [Theory]
        [Trait(Traits.Execution, Traits.Automatic)]
        [InlineData("Status1", "", new[] { "Status1" }, new string[] { }, true)]
        [InlineData("Status1", "Type1", new[] { "Status1" }, new string[] { }, true)]
        [InlineData("Status2", "", new[] { "Status1" }, new string[] { }, false)]
        [InlineData("", "Type1", new string[] { }, new[] { "Type1" }, true)]
        [InlineData("Status1", "Type1", new string[] { }, new[] { "Type1" }, true)]
        [InlineData("", "Type1", new string[] { }, new[] { "Type2" }, false)]
        [InlineData("Status2", "", new[] { "Status1", "Status2" }, new string[0], true)]
        [InlineData("Status2", "Type1", new[] { "Status1", "Status2" }, new[] { "Type1" }, true)]
        [InlineData("", "", new string[] { }, new[] { "Type1" }, false)]
        [InlineData("Status1", "", new string[] { }, new[] { "Type1" }, false)]
        public void TestShouldSynchronise(
            string status,
            string type,
            IEnumerable<string> acceptedStatuses,
            IEnumerable<string> acceptedTypes,
            bool expectedResult)
        {
            var organisation = new OrganisationDto();
            organisation.OrganisationStatus = status;
            organisation.OrganisationType = type;

            var configuration = new PowerofficeConfiguration();
            configuration.AcceptedOrganisationStatuses = acceptedStatuses;
            configuration.AcceptedOrganisationTypes = acceptedTypes;

            bool actualResult = organisation.ShouldSynchronise(configuration);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}