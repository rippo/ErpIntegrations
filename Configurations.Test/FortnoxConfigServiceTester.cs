using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Test;
using Xunit;

namespace Webcrm.ErpIntegrations.Configurations.Test
{
    public class FortnoxConfigServiceTester
    {
        public FortnoxConfigServiceTester()
        {
            // It's safe to use .Result in the tests.
            ConfigService = Task.Run(() => FortnoxConfigService.Create(TestTypedEnvironment.DatabaseCredentials)).Result;
        }

        private FortnoxConfigService ConfigService { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadFortnoxConfiguration()
        {
            var configuration = ConfigService.LoadFortnoxConfiguration(TestConfigurations.WebcrmSystemId);

            Assert.NotNull(configuration);
            Assert.Equal(TestConfigurations.FortnoxOrganisationIdFieldName, configuration.OrganisationIdFieldName);
            Assert.Equal(TestConfigurations.FortnoxAccessToken, configuration.FortnoxAccessToken);
            Assert.Equal(TestConfigurations.FortnoxClientSecret, configuration.FortnoxClientSecret);
            Assert.Equal(TestConfigurations.WebcrmApiKey, configuration.WebcrmApiKey);
        }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadFortnoxConfigurations()
        {
            var configurations = ConfigService.LoadEnabledFortnoxConfigurations();

            Assert.NotNull(configurations);
            Assert.True(configurations.Any());
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestSaveFortnoxConfiguration()
        {
            await ConfigService.SaveFortnoxConfiguration(TestConfigurations.FortnoxConfiguration);
        }
    }
}