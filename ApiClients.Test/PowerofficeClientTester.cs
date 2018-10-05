using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class PowerofficeClientTester : BaseTester
    {
        public PowerofficeClientTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            Client = Task.Run(() => PowerofficeClient.Create(
                TestTypedEnvironment.PowerofficeApiSettings,
                TestConfigurations.PowerofficeConfiguration.PowerofficeClientKey)).Result;
        }

        private PowerofficeClient Client { get; }
        private const long CompanyIdForTestingCrudOperationsOnContactPersons = 3495003;

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateUpdateDeleteInvoice()
        {
            var newInvoice = new NewOutgoingInvoice
            {
                CustomerCode = 1522391,
                Status = OutgoingInvoiceStatus.Approved
            };
            newInvoice.OutgoingInvoiceLines.Add(new OutgoingInvoiceLine
            {
                Description = "Created by automated test"
            });

            var createdInvoice = await Client.CreateInvoice(newInvoice);
            Assert.Equal(newInvoice.Status, createdInvoice.Status);
            Assert.True(createdInvoice.OutgoingInvoiceLines.Count == 1);
            Assert.Equal(newInvoice.OutgoingInvoiceLines[0].Description, createdInvoice.OutgoingInvoiceLines[0].Description);

            createdInvoice.Status = OutgoingInvoiceStatus.Draft;
            var updatedInvoice = await Client.UpdateInvoice(createdInvoice);
            Assert.Equal(createdInvoice.Id, updatedInvoice.Id);
            Assert.Equal(createdInvoice.Status, updatedInvoice.Status);

            await Client.DeleteInvoice(createdInvoice.Id);
            await Assert.ThrowsAsync<ApplicationException>(async () => await Client.GetInvoice(createdInvoice.Id));
        }

        [Theory]
        [Trait(Traits.Execution, Traits.Manual)]
        [InlineData(CompanyIdForTestingCrudOperationsOnContactPersons)]
        public async Task TestCreateReadUpdateDeleteContactPerson(long customerId)
        {
            var newContactPerson = new NewContactPerson
            {
                FirstName = "Created by an automated test",
                LastName = "(Should have been deleted)"
            };

            var createdContactPerson = await Client.CreateContactPerson(customerId, newContactPerson);
            Assert.Equal(newContactPerson.FirstName, createdContactPerson.FirstName);
            Assert.Equal(newContactPerson.LastName, createdContactPerson.LastName);

            var readContactPerson = await Client.GetContactPerson(customerId, createdContactPerson.Id);
            Assert.Equal(createdContactPerson.Id, readContactPerson.Id);
            Assert.Equal(createdContactPerson.FirstName, readContactPerson.FirstName);
            Assert.Equal(createdContactPerson.LastName, readContactPerson.LastName);

            createdContactPerson.FirstName = "Created and updated by an automated test";
            var updatedContactPerson = await Client.UpdateContactPerson(customerId, createdContactPerson);
            Assert.Equal(createdContactPerson.Id, updatedContactPerson.Id);
            Assert.Equal(createdContactPerson.FirstName, updatedContactPerson.FirstName);
            Assert.Equal(createdContactPerson.LastName, updatedContactPerson.LastName);

            await Client.DeleteContactPerson(customerId, updatedContactPerson.Id);
            var deletedContactPerson = await Client.GetContactPerson(customerId, updatedContactPerson.Id);
            Assert.Null(deletedContactPerson);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateReadUpdateDeleteCustomer()
        {
            var newCustomer = new NewCustomer
            {
                Name = "Created by an automated test (should also have been deleted)"
            };

            var createdCustomer = await Client.CreateCustomer(newCustomer);
            Assert.Equal(newCustomer.Name, createdCustomer.Name);

            var readCustomer = await Client.GetCustomer(createdCustomer.Id);
            Assert.Equal(createdCustomer.Name, readCustomer.Name);

            createdCustomer.Name = "Created and updated by an automated test (should also have been deleted)";
            var updatedCustomer = await Client.UpdateCustomer(createdCustomer);
            Assert.Equal(createdCustomer.Name, updatedCustomer.Name);

            await Client.DeleteCustomer(updatedCustomer.Id);
            var deletedCustomer = await Client.GetCustomer(updatedCustomer.Id);
            Assert.Null(deletedCustomer);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetProductByCode()
        {
            const string productCode = "8";
            var product = await Client.GetProductByCode(productCode);

            Assert.NotNull(product);
            Assert.Equal(2059019, product.Id);
            Assert.Equal("Cola", product.Name);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedInvoices()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var upsertedInvoices = await Client.GetUpsertedInvoices(longAgo);

            Assert.True(upsertedInvoices.Count >= 2);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisations()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var customers = await Client.GetUpsertedOrganisations(longAgo);

            Assert.True(customers.Count >= 2);

            foreach (var customer in customers)
            {
                Assert.True(customer.Id >= 1);
                Assert.NotNull(customer.Name);
                Assert.True(customer.Name.Length >= 1);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedProducts()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var products = await Client.GetUpsertedProducts(longAgo);

            Assert.True(products.Count >= 2);

            foreach (var product in products)
            {
                Assert.True(product.Id >= 1);
                Assert.NotNull(product.Name);
                Assert.True(product.Name.Length >= 1);
                Assert.NotNull(product.Code);
                Assert.True(product.Code.Length >= 1);
            }
        }
    }
}