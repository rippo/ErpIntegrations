using System;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Test;
using Xunit;

namespace Webcrm.ErpIntegrations.Configurations.Test
{
    public class PowerofficeConfigServiceTester
    {
        public PowerofficeConfigServiceTester()
        {
            // It's safe to use .Result in the tests.
            ConfigService = Task.Run(() => PowerofficeConfigService.Create(TestTypedEnvironment.DatabaseCredentials)).Result;
        }

        private PowerofficeConfigService ConfigService { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateReadUpdateDeleteConfiguration()
        {
            var now = DateTime.UtcNow;
            const string systemId = "TestSystemId";
            const string originalPersonIdFieldName = "OriginalPersonIdFieldName";
            const string updatedPersonIdFieldName = "UpdatedPersonIdFieldName";

            var testConfiguration = new PowerofficeConfiguration
            {
                AcceptedOrganisationStatuses = new [] { "AcceptedOrganisationStatuses1", "AcceptedOrganisationStatuses2" },
                AcceptedOrganisationTypes = new[] { "AcceptedOrganisationTypes1", "AcceptedOrganisationTypes2" },
                DeliveryIdFieldName = "DeliveryIdFieldName",
                Disabled = true,
                LastSuccessfulCopyFromErpHeartbeat = now,
                LastSuccessfulCopyToErpHeartbeat = now,
                OrganisationIdFieldName = "OrganisationIdFieldName",
                PersonIdFieldName = originalPersonIdFieldName,
                PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                ProductCodeFieldName = "ProductCodeFieldName",
                ProductIdFieldName = "ProductIdFieldName",
                ProductNameFieldName = "ProductNameFieldName",
                WebcrmApiKey = "WebcrmApiKey",
                WebcrmSystemId = systemId
            };

            await ConfigService.SavePowerofficeConfiguration(testConfiguration);
            var loadedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            Assert.NotNull(loadedConfiguration);
            Assert.True(loadedConfiguration.Disabled);
            Assert.Equal(originalPersonIdFieldName, loadedConfiguration.PersonIdFieldName);
            Assert.Equal(systemId, loadedConfiguration.WebcrmSystemId);

            loadedConfiguration.PersonIdFieldName = updatedPersonIdFieldName;
            await ConfigService.SavePowerofficeConfiguration(loadedConfiguration);
            var updatedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            Assert.Equal(updatedPersonIdFieldName, updatedConfiguration.PersonIdFieldName);

            await ConfigService.DeletePowerofficeConfiguration(updatedConfiguration);
            var deletedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            Assert.Null(deletedConfiguration);
        }

        /// <summary>This test verifies that the PowerOffice configuration stored in Azure matches the one hardcoded in `TestConfigurations`. The test `SystemConfiguratorTester.TestPowerofficeSetup` copies the values in `TestConfigurations` to the database in Azure.</summary>
        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadPowerofficeConfiguration()
        {
            var configuration = ConfigService.LoadPowerofficeConfiguration(TestConfigurations.WebcrmSystemId);

            Assert.NotNull(configuration);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.AcceptedOrganisationStatuses, configuration.AcceptedOrganisationStatuses);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.AcceptedOrganisationTypes, configuration.AcceptedOrganisationTypes);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.DeliveryIdFieldName, configuration.DeliveryIdFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.OrganisationIdFieldName, configuration.OrganisationIdFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.PersonIdFieldName, configuration.PersonIdFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.PowerofficeClientKey, configuration.PowerofficeClientKey);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.ProductIdFieldName, configuration.ProductIdFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.ProductNameFieldName, configuration.ProductNameFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.ProductSalesAccountFieldName, configuration.ProductSalesAccountFieldName);
            Assert.Equal(TestConfigurations.PowerofficeConfiguration.ProductUnitFieldName, configuration.ProductUnitFieldName);
            Assert.Equal(TestConfigurations.WebcrmApiKey, configuration.WebcrmApiKey);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadEnabledPowerofficeConfigurations()
        {
            var configurations = ConfigService.LoadEnabledPowerofficeConfigurations();

            Assert.NotNull(configurations);
            Assert.True(configurations.Any());
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestSavePowerofficeConfiguration()
        {
            await ConfigService.SavePowerofficeConfiguration(TestConfigurations.PowerofficeConfiguration);
        }
    }
}