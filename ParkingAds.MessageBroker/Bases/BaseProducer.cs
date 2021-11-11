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
        private IConnection _connection;
        private IModel _channel;
        private readonly static LogProducer _logger = new();
        private readonly string _queueName;

        public BaseProducer(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null) : base(queueName, isDurable, isExclusive, shouldAutoDelete, queueArguments)
        {
            _queueName = queueName;
        }

        public virtual void SendMessage(TBody input)
        {
            Guid corId = LogMessage.GenerateCorrelationId();
            TryCreateQueue(ref _connection, ref _channel);
            LogMessage logMessage = new($"Trying to send message to {_queueName}", corId);
            using (_connection)
            using (_channel)
            {
                string jsonSerializedTBody = JsonConvert.SerializeObject(input);
                byte[] body = Encoding.UTF8.GetBytes(jsonSerializedTBody);
                _channel.BasicPublish(exchange: string.Empty, routingKey: _queueName, basicProperties: null, body: body);
                logMessage.AddMessageToLogChain("Message sent successfully");
                _logger.SendMessage(logMessage);
            }
        }
    }
}
