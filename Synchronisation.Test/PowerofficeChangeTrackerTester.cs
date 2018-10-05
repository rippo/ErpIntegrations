using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class PowerofficeChangeTrackerTester : BaseTester
    {
        public PowerofficeChangeTrackerTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ChangeTracker = Task.Run(() => PowerofficeChangeTracker.Create(
                OutputLogger,
                TestTypedEnvironment.AzureWebJobsStorage,
                TestPowerofficeClientFactory)).Result;
        }

        private PowerofficeChangeTracker ChangeTracker { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedItemsForOneSystem()
        {
            var anHourAgo = DateTime.UtcNow.AddHours(-1);
            await ChangeTracker.EnqueueUpsertedItemsForOneSystem(anHourAgo, TestConfigurations.PowerofficeConfiguration);
        }
    }
}