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

        private void TryCreateQueue(out IConnection connection, out IModel channel, string queueName)
        {
            ConnectionFactory factory = new()
            {
                HostName = DefaultQueueHostname,
                Port = DefaultQueuePort,
                UserName = DefaultQueueUsername,
                Password = DefaultQueuePassword
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public override void SendMessage(LogMessage message)
        {
            string queueName = message.LogType switch
            {
                LogType.DEBUG => QueueNames.DebugLogQueue,
                LogType.INFO => QueueNames.InfoLogQueue,
                LogType.ERROR => QueueNames.ErrorLogQueue,
                LogType.WARN => QueueNames.WarnLogQueue,
                LogType.TRACE => QueueNames.TraceLogQueue,
                _ => QueueNames.DefaultLogQueue,
            };
            TryCreateQueue(out IConnection connection, out IModel channel, queueName);
            using (connection)
            using (channel)
            {
                string jsonSerializedTBody = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(jsonSerializedTBody);
                channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            }

            if (message.GetLogChain().Count > 0)
                foreach (var item in message.GetLogChain())
                    SendMessage(item);
        }
    }
}
