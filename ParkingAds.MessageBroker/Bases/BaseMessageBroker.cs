using Newtonsoft.Json;
using ParkingAds.MessageBroker.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Bases
{
    public abstract class BaseMessageBroker<TBody> : IMessageBroker
    {
        private readonly string _queueName;
        private readonly bool _isDurable;
        private readonly bool _isExclusive;
        private readonly bool _shouldAutoDelete;
        private readonly IDictionary<string, object> _queueArguments;

        public virtual string QueueHostname { get; set; }
        public virtual int QueuePort { get; set; } = 5672; //defualt port
        public virtual string QueueUsername { get; set; }
        public virtual string QueuePassword { get; set; }
        public readonly ConnectionFactory _factory;

        public BaseMessageBroker(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null)
        {
            _queueName = queueName;
            _isDurable = isDurable;
            _isExclusive = isExclusive;
            _shouldAutoDelete = shouldAutoDelete;
            _queueArguments = queueArguments;

            QueueHostname = ConfigurationManager.AppSettings["HostName"];
            if (int.TryParse(ConfigurationManager.AppSettings["Port"], out int port)) QueuePort = port;
            QueueUsername = ConfigurationManager.AppSettings["UserName"];
            QueuePassword = ConfigurationManager.AppSettings["Password"];
            _factory = new()
            {
                HostName = QueueHostname,
                Port = QueuePort,
                UserName = QueueUsername,
                Password = QueuePassword
            };
        }

        public BaseMessageBroker()
        {
            QueueHostname = ConfigurationManager.AppSettings["HostName"];
            if (int.TryParse(ConfigurationManager.AppSettings["Port"], out int port)) QueuePort = port;
            QueueUsername = ConfigurationManager.AppSettings["UserName"];
            QueuePassword = ConfigurationManager.AppSettings["Password"];
            _factory = new()
            {
                HostName = QueueHostname,
                Port = QueuePort,
                UserName = QueueUsername,
                Password = QueuePassword
            };
        }

        public virtual void TryCreateQueue(IModel channel, string queueName = "")
        {
            try
            {
                channel.QueueDeclare(queue: string.IsNullOrEmpty(queueName) ? _queueName : queueName, durable: _isDurable, exclusive: _isExclusive, autoDelete: _shouldAutoDelete, arguments: _queueArguments);
            }
            catch (Exception e)
            {
                Console.WriteLine($"EXCEPTION: {e.Message}");
            }
        }
    }
}
