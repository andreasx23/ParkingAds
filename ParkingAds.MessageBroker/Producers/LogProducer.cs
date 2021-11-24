using Newtonsoft.Json;
using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageBroker.Resx;
using ParkingAds.MessageModel;
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
        public LogProducer() : base(QueueNames.DefaultLog, true, false, false, null)
        {
        }

        private void TryCreateQueue(out IConnection connection, out IModel channel, string queueName)
        {
            ConnectionFactory factory = new()
            {
                HostName = QueueHostname,
                Port = QueuePort,
                UserName = QueueUsername,
                Password = QueuePassword
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public override void SendMessage(LogMessage message)
        {
            string queueName = message.LogType switch
            {
                LogType.DEBUG => QueueNames.DebugLog,
                LogType.INFO => QueueNames.InfoLog,
                LogType.ERROR => QueueNames.ErrorLog,
                LogType.WARN => QueueNames.WarnLog,
                LogType.TRACE => QueueNames.TraceLog,
                _ => QueueNames.DefaultLog,
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
