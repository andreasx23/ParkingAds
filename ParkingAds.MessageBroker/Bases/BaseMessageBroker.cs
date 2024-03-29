﻿using Newtonsoft.Json;
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
        private string _queueName;
        private bool _isDurable;
        private bool _isExclusive;
        private bool _shouldAutoDelete;
        private IDictionary<string, object> _queueArguments;

        public virtual string QueueHostname { get; set; }
        public virtual int QueuePort { get; set; } = 5672; //defualt port
        public virtual string QueueUsername { get; set; }
        public virtual string QueuePassword { get; set; }

        public readonly ConnectionFactory _factory;
        public readonly IConnection _connection;
        public readonly IModel _channel;

        public BaseMessageBroker(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null)
        {
            _queueName = queueName;
            _isDurable = isDurable;
            _isExclusive = isExclusive;
            _shouldAutoDelete = shouldAutoDelete;
            _queueArguments = queueArguments;

            QueueHostname = ConfigurationManager.AppSettings["MessageBrokerHostName"];
            if (int.TryParse(ConfigurationManager.AppSettings["MessageBrokerPort"], out int port)) QueuePort = port;
            QueueUsername = ConfigurationManager.AppSettings["MessageBrokerUserName"];
            QueuePassword = ConfigurationManager.AppSettings["MessageBrokerPassword"];
            _factory = new()
            {
                HostName = QueueHostname,
                Port = QueuePort,
                UserName = QueueUsername,
                Password = QueuePassword
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
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
