using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class PowerofficeDataCopierTester : BaseTester
    {
        public PowerofficeDataCopierTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            DataCopier = Task.Run(() => PowerofficeDataCopier.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestPowerofficeClientFactory,
                TestConfigurations.PowerofficeConfiguration)).Result;
        }

        private PowerofficeDataCopier DataCopier { get; }
        private const int WebcrmOrganisationUsedByIntegrationTestsId = 13;
        private const int WebcrmPersonUsedByIntegrationTestsId = 7;
        private const long PowerofficeOrganisationUsedByIntegrationTestsId = 3495003;
        private const long PowerofficePrimaryContactPersonId = 1185003;

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCopyOrganisationFromPoweroffice()
        {
            await DataCopier.CopyOrganisationFromPoweroffice(PowerofficeOrganisationUsedByIntegrationTestsId);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCopyOrganisationToPoweroffice()
        {
            await DataCopier.CopyOrganisationToPoweroffice(WebcrmOrganisationUsedByIntegrationTestsId);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCopyPersonFromPoweroffice()
        {
            await DataCopier.CopyPersonFromPoweroffice(PowerofficeOrganisationUsedByIntegrationTestsId, PowerofficePrimaryContactPersonId);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCopyPersonToPoweroffice()
        {
            await DataCopier.CopyPersonToPoweroffice(WebcrmPersonUsedByIntegrationTestsId);
        }
    }
}