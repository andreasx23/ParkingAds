using Newtonsoft.Json;
using ParkingAds.MessageBroker.Interfaces;
using ParkingAds.MessageBroker.Producers;
using ParkingAds.Model;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Bases
{
    public abstract class BaseProducer<TBody> : BaseMessageBroker<TBody>, IProducer<TBody>
    {
        private readonly static LogProducer _logger = new();

        public string QueueName { get; set; }

        public BaseProducer(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null) : base(queueName, isDurable, isExclusive, shouldAutoDelete, queueArguments)
        {
            QueueName = queueName;
        }

        public virtual void SendMessage(TBody input)
        {
            Guid corId = LogMessage.GenerateCorrelationId();
            TryCreateQueue(out IConnection connection, out IModel channel);
            LogMessage logMessage = new($"Trying to send message to {QueueName}", corId);
            using (connection)
            using (channel)
            {
                string jsonSerializedTBody = JsonConvert.SerializeObject(input);
                byte[] body = Encoding.UTF8.GetBytes(jsonSerializedTBody);
                channel.BasicPublish(exchange: string.Empty, routingKey: QueueName, basicProperties: null, body: body);
                logMessage.AddMessageToLogChain("Message sent successfully");
                _logger.SendMessage(logMessage);
            }
        }
    }
}
