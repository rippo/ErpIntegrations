using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation.Models;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    internal sealed class FortnoxQueue
    {
        private FortnoxQueue(string storageAccountConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            Queue = queueClient.GetQueueReference(Constants.FortnoxQueueName);
        }

        public static async Task<FortnoxQueue> Create(string storageAccountConnectionString)
        {
            var fortnoxQueue = new FortnoxQueue(storageAccountConnectionString);
            await fortnoxQueue.Queue.CreateIfNotExistsAsync();
            return fortnoxQueue;
        }

        private CloudQueue Queue { get; }

        internal async Task Enqueue(FortnoxQueueMessage message)
        {
            string serializedMessage = JsonConvert.SerializeObject(message);
            var queueMessage = new CloudQueueMessage(serializedMessage);
            await Queue.AddMessageAsync(queueMessage);
        }
    }
}