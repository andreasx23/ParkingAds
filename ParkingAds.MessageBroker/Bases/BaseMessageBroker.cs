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
        private readonly ConnectionFactory _factory;

        public virtual string DefaultQueueHostname { get; set; }
        public virtual int DefaultQueuePort { get; set; } = 5672; //defualt port
        public virtual string DefaultQueueUsername { get; set; }
        public virtual string DefaultQueuePassword { get; set; }

        public BaseMessageBroker(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null)
        {
            _queueName = queueName;
            _isDurable = isDurable;
            _isExclusive = isExclusive;
            _shouldAutoDelete = shouldAutoDelete;
            _queueArguments = queueArguments;

            DefaultQueueHostname = ConfigurationManager.AppSettings["HostName"];
            if (int.TryParse(ConfigurationManager.AppSettings["Port"], out int port)) DefaultQueuePort = port;
            DefaultQueueUsername = ConfigurationManager.AppSettings["UserName"];
            DefaultQueuePassword = ConfigurationManager.AppSettings["Password"];
            _factory = new()
            {
                HostName = DefaultQueueHostname,
                Port = DefaultQueuePort,
                UserName = DefaultQueueUsername,
                Password = DefaultQueuePassword
            };
        }

        public virtual void TryCreateQueue(out IConnection connection, out IModel channel)
        {
            connection = _factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: _isDurable, exclusive: _isExclusive, autoDelete: _shouldAutoDelete, arguments: _queueArguments);
        }
    }
}
