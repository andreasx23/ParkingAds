using Newtonsoft.Json;
using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageBroker.Producers;
using ParkingAds.MessageModel;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Consumers
{
    public class WiretapConsumer : BaseConsumer<string>
    {
        private readonly WiretapProducer _producer;

        public WiretapConsumer() : base(Resx.QueueNames.Wiretap, true, false, false, null)
        {
            _producer = new();
        }

        public void ConsumeWiretapMessages()
        {
            TryCreateQueue(_channel);
            BasicGetResult msg = _channel.BasicGet(QueueName, false);
            if (msg == null) return;
            _channel.BasicAck(msg.DeliveryTag, false);
            byte[] body = msg.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);
            if (!HasWiretapObject(message)) throw new Exception(); //TODO HANDLE THIS
            if (isWiretapEnabled()) WriteToFile(message);
            WiretapMessage wiretap = RetrieveWiretapMessage(message);
            _producer.SendMessage(message, wiretap.QueueDestination);
        }

        private void WriteToFile(string objectToWrite) //TODO IMPLEMENT WRITE TO FILE
        {
            Console.WriteLine("Simulating writing to file..");
            Console.WriteLine(objectToWrite);
        }

        private WiretapMessage RetrieveWiretapMessage(string message)
        {
            dynamic dynamic = JsonConvert.DeserializeObject<dynamic>(message);
            dynamic dynamicWiretapString = dynamic.WiretapMessage.ToString();
            WiretapMessage wiretapMessage = JsonConvert.DeserializeObject<WiretapMessage>(dynamicWiretapString);
            return wiretapMessage;
        }

        private bool HasWiretapObject(string message)
        {
            try
            {
                dynamic dynamic = JsonConvert.DeserializeObject<dynamic>(message);
                dynamic wire = dynamic.WiretapMessage.ToString(); //If I cant do this step it will throw an exception
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool isWiretapEnabled()
        {
            string isWiretapEnabled = ConfigurationManager.AppSettings["EnableWiretap"].ToLower();
            return isWiretapEnabled == "true";
        }
    }
}
