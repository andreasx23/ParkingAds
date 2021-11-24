using Newtonsoft.Json;
using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageModel;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Producers
{
    public class WiretapProducer : BaseProducer<string>
    {
        private readonly static LogProducer _logger = new();

        public WiretapProducer() : base(string.Empty, true, false, false, null)
        {
        }

        /// <summary>
        /// Send a message to a specified queue
        /// </summary>
        /// <param name="input">The data to send</param>
        /// <param name="queueName">The queue to target</param>
        public virtual void SendMessage(string input, string queueName)
        {
            Guid corId = LogMessage.GenerateCorrelationId();
            LogMessage logMessage = new($"Trying to send message to {queueName}", corId);
            TryCreateQueue(_channel, queueName);
            byte[] body = Encoding.UTF8.GetBytes(input);
            _channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            logMessage.AddMessageToLogChain("Message sent successfully");
            _logger.SendMessage(logMessage);
        }
    }
}
