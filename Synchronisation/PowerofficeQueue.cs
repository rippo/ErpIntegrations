using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.GeneralUtilities;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    internal sealed class PowerofficeQueue
    {
        private PowerofficeQueue(
            ILogger logger,
            string storageAccountConnectionString)
        {
            Logger = logger;
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            Queue = queueClient.GetQueueReference(Constants.PowerofficeQueueName);
        }

        public static async Task<PowerofficeQueue> Create(
            ILogger logger,
            string storageAccountConnectionString)
        {
            var powerofficeQueue = new PowerofficeQueue(logger, storageAccountConnectionString);
            await powerofficeQueue.Queue.CreateIfNotExistsAsync();
            return powerofficeQueue;
        }

        private ILogger Logger { get; }
        private CloudQueue Queue { get; }

        /// <summary>Get the first message on the queue and delete it. Dequeuing is only used by the tests.</summary>
        public async Task<PowerofficeQueueMessage> Dequeue()
        {
            var genericMessage = await Queue.GetMessageAsync();
            if (genericMessage == null)
                return null;

            await Queue.DeleteMessageAsync(genericMessage);

            var typedMessage = JsonConvert.DeserializeObject<PowerofficeQueueMessage>(genericMessage.AsString);
            return typedMessage;
        }

        /// <summary>Get the first message on the queue without removing it. Dequeuing is only used by the tests.</summary>
        public async Task<PowerofficeQueueMessage> Peek()
        {
            var genericMessage = await Queue.GetMessageAsync();
            if (genericMessage == null)
                return null;

            var typedMessage = JsonConvert.DeserializeObject<PowerofficeQueueMessage>(genericMessage.AsString);
            return typedMessage;
        }

        public async Task Enqueue(PowerofficeQueueMessage message)
        {
            string serializedMessage = JsonConvert.SerializeObject(message);
            Logger.LogTrace($"Adding message to the PowerOffice queue. Content: {serializedMessage.Truncate(2000, addEllipsis: true)}.");
            var queueMessage = new CloudQueueMessage(serializedMessage);
            await Queue.AddMessageAsync(queueMessage);
        }
    }
}