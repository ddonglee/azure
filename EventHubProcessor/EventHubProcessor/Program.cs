using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;

using Microsoft.ServiceBus.Messaging;

using Microsoft.WindowsAzure.Storage;


namespace EventHubProcessor
{
    class Program
    {
        public static Func<object, DateTime> InitialOffsetProvider { get; private set; }

        static void Main(string[] args)
        {

            string eventhubConnectionString = @"";
            string eventhubName = "messages/events";
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}");

            string _guId = Guid.NewGuid().ToString();

            string eventProcessorHostName = _guId;
            string leaseName = eventProcessorHostName = _guId;

            EventProcessorHost eventProcessorHost = new EventProcessorHost(
                eventProcessorHostName,
                eventhubName,
                EventHubConsumerGroup.DefaultGroupName,
                eventhubConnectionString,
                storageConnectionString,
                leaseName);
            Console.WriteLine("eventProcessorHost");

            var opions = new EventProcessorOptions();
            {
                InitialOffsetProvider = (partitionId) => DateTime.UtcNow;
            };
            opions.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(opions).Wait();
            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }

        private static void Opions_ExceptionReceived(object sender, ExceptionReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
