using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class WebcrmClientTester : BaseTester
    {
        public WebcrmClientTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            Client = Task.Run(() => WebcrmClient.Create(
                OutputLogger,
                TestTypedEnvironment.WebcrmApiBaseUrl,
                TestConfigurations.WebcrmApiKey)).Result;
        }

        private WebcrmClient Client { get; }

        [Theory]
        [Trait(Traits.Execution, Traits.Automatic)]
        [InlineData(1, "webCRM")]
        public async Task TestGetOrganisationById(int organisationId, string organisationName)
        {
            var organisation = await Client.GetOrganisationById(organisationId);
            Assert.Equal(organisationName, organisation.OrganisationName);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedDeliveries()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            var upsertedDeliveries = await Client.GetUpsertedDeliveries(longAgo);
            Assert.True(upsertedDeliveries != null);
            Assert.True(upsertedDeliveries.Count >= 2);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisationsByStatus()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            string[] acceptedStatuses = { "A Kunde", "B Kunde" };
            var upsertedOrganisations = await Client.GetUpsertedOrganisations(longAgo, acceptedStatuses, null);
            Assert.True(upsertedOrganisations != null);
            Assert.True(upsertedOrganisations.Count >= 2);
            foreach (var organisation in upsertedOrganisations)
            {
                Assert.Contains(organisation.OrganisationStatus, acceptedStatuses);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisationsByType()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            string[] acceptedTypes = { "Leverandør", "Partner" };
            var upsertedOrganisations = await Client.GetUpsertedOrganisations(longAgo, null, acceptedTypes);
            Assert.True(upsertedOrganisations != null);
            Assert.True(upsertedOrganisations.Count >= 2);
            foreach (var organisation in upsertedOrganisations)
            {
                Assert.Contains(organisation.OrganisationType, acceptedTypes);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedPersons()
        {
            var longAgo = DateTime.Now.AddDays(-1000);
            var upsertedPersons = await Client.GetUpsertedPersons(longAgo, TestConfigurations.PowerofficeConfiguration);
            Assert.True(upsertedPersons != null);
            Assert.True(upsertedPersons.Count >= 2);
        }
    }
}