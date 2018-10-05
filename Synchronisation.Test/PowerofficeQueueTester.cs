using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class PowerofficeQueueTester : BaseTester
    {
        public PowerofficeQueueTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            Dispatcher = PowerofficeMessageDispatcher.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestTypedEnvironment.DatabaseCredentials,
                TestPowerofficeClientFactory).Result;
            Queue = PowerofficeQueue.Create(OutputLogger, TestTypedEnvironment.AzureWebJobsStorage).Result;
        }

        private PowerofficeMessageDispatcher Dispatcher { get; }
        private PowerofficeQueue Queue { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestDequeuingAllMessages()
        {
            var message = await Queue.Dequeue();
            try
            {
                while (message != null)
                {
                    await Dispatcher.HandleDequeuedMessage(message);
                    message = await Queue.Dequeue();
                }
            }
            catch (Exception)
            {
                // The failed message will be added to the end of the queue.
                await Queue.Enqueue(message);
                throw;
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestDequeuingFirstMessage()
        {
            var message = await Queue.Peek();

            if (message == null)
                throw new ApplicationException("This test method expects at least one message in the queue.");

            await Dispatcher.HandleDequeuedMessage(message);
        }
    }
}