using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class FortnoxChangeTrackerTester : BaseTester
    {
        public FortnoxChangeTrackerTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ChangeTracker = Task.Run(() => FortnoxChangeTracker.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestTypedEnvironment.AzureWebJobsStorage,
                TestTypedEnvironment.DatabaseCredentials)).Result;
        }

        private FortnoxChangeTracker ChangeTracker { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedCustomers()
        {
            await ChangeTracker.EnqueueUpsertedCustomers();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedInvoices()
        {
            await ChangeTracker.EnqueueUpsertedInvoices();
        }
    }
}