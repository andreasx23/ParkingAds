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

        public BaseMessageBroker(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null)
        {
            _queueName = queueName;
            _isDurable = isDurable;
            _isExclusive = isExclusive;
            _shouldAutoDelete = shouldAutoDelete;
            _queueArguments = queueArguments;

            string hostName = ConfigurationManager.AppSettings["HostName"];
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out int port)) port = 5672; //defualt port
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            _factory = new() { HostName = hostName, Port = port, UserName = userName, Password = password };
        }

        public virtual void CreateQueue(out IConnection connection, out IModel channel)
        {
            connection = _factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: _isDurable, exclusive: _isExclusive, autoDelete: _shouldAutoDelete, arguments: _queueArguments);
        }
    }
}
