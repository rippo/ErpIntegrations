using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class FortnoxDataCopierTester : BaseTester
    {
        public FortnoxDataCopierTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            DataCopier = Task.Run(() => FortnoxDataCopier.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestConfigurations.FortnoxConfiguration)).Result;
        }

        private FortnoxDataCopier DataCopier { get; }

        [Theory]
        [InlineData("9", TestConfigurations.FortnoxOrganisationIdFieldName)]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCopySingleOrganisationFromFortnox(string fortnoxCustomerId, string organisationIdFieldName)
        {
            await DataCopier.CopyOrganisationFromFortnox(fortnoxCustomerId, organisationIdFieldName);
        }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueRecentlyUpsertedInvoices()
        {
            var dateSinceUpserted = new DateTime(2018, 1, 1);

            await DataCopier.EnqueueRecentlyUpsertedInvoices(TestTypedEnvironment.AzureWebJobsStorage, TestConfigurations.WebcrmSystemId, dateSinceUpserted);
        }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueRecentlyUpsertedCustomers()
        {
            var dateSinceUpserted = new DateTime(2018, 1, 1);

            await DataCopier.EnqueueRecentlyUpsertedCustomers(TestTypedEnvironment.AzureWebJobsStorage, TestConfigurations.WebcrmSystemId, dateSinceUpserted);
        }
    }
}