using Newtonsoft.Json;
using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageBroker.Resx;
using ParkingAds.Model;
using ParkingAds.Model.Enums;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Producers
{
    public class LogProducer : BaseProducer<LogMessage>
    {
        public LogProducer() : base(QueueNames.DefaultLogQueue, true, false, false, null)
        {
        }

        public override void SendMessage(LogMessage message)
        {
            string queueName = GetType().Name;
            switch (message.LogType)
            {
                case LogType.DEBUG:
                    queueName = QueueNames.DebugLogQueue;
                    break;
                case LogType.INFO:
                    queueName = QueueNames.InfoLogQueue;
                    break;
                case LogType.ERROR:
                    queueName = QueueNames.ErrorLogQueue;
                    break;
                case LogType.WARN:
                    queueName = QueueNames.WarnLogQueue;
                    break;
                case LogType.TRACE:
                    queueName = QueueNames.TraceLogQueue;
                    break;
            }

            ConnectionFactory factory = new() { };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            using (connection)
            using (channel)
            {
                string jsonSerializedTBody = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(jsonSerializedTBody);
                channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            }

            if (message.GetLogChain().Count > 0)
            {
                foreach (var item in message.GetLogChain())
                {
                    SendMessage(item);
                }
            }
        }
    }
}
