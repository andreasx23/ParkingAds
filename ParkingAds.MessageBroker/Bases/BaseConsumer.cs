using Newtonsoft.Json;
using ParkingAds.MessageBroker.Interfaces;
using ParkingAds.MessageBroker.Producers;
using ParkingAds.MessageModel;
using ParkingAds.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Bases
{
    public abstract class BaseConsumer<TBody> : BaseMessageBroker<TBody>, IConsumer<TBody>
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly static LogProducer _logger = new();
        public string QueueName { get; set; }

        public BaseConsumer(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null) : base(queueName, isDurable, isExclusive, shouldAutoDelete, queueArguments)
        {
            QueueName = queueName;
        }

        public virtual TBody ConsumeMessageWithPolling()
        {
            Guid corId = LogMessage.GenerateCorrelationId();
            TryCreateQueue(ref _connection, ref _channel);
            LogMessage logMessage = new($"Trying to consume message from {QueueName} using polling", corId);

            using (_connection)
            using (_channel)
            {
                BasicGetResult msg = _channel.BasicGet(QueueName, false);
                while (msg == null)
                {
                    const int sleepOneSecondInMs = 1000;
                    Thread.Sleep(sleepOneSecondInMs);
                    msg = _channel.BasicGet(QueueName, false);
                }
                byte[] body = msg.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                try
                {
                    TBody jsonDeserializedTBody = JsonConvert.DeserializeObject<TBody>(message);
                    _channel.BasicAck(msg.DeliveryTag, false);
                    logMessage.AddMessageToLogChain("Message consumed successfully");
                    return jsonDeserializedTBody;
                }
                catch (Exception e)
                {
                    logMessage.AddMessageToLogChain($"An exception has occured returning default value of {typeof(TBody)}", e, e.StackTrace);
                    return default;
                }
                finally
                {
                    if (typeof(TBody) != typeof(LogMessage)) _logger.SendMessage(logMessage);
                }
            }
        }

        public TBody ConsumeMessage()
        {
            Guid corId = LogMessage.GenerateCorrelationId();
            TryCreateQueue(ref _connection, ref _channel);
            LogMessage logMessage = new($"Trying to consume message from {QueueName}", corId);

            using (_connection)
            using (_channel)
            {
                try
                {
                    BasicGetResult msg = _channel.BasicGet(QueueName, false);
                    if (msg == null) return default;
                    byte[] body = msg.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);
                    TBody jsonDeserializedTBody = JsonConvert.DeserializeObject<TBody>(message);
                    _channel.BasicAck(msg.DeliveryTag, false);
                    logMessage.AddMessageToLogChain("Message consumed successfully");
                    return jsonDeserializedTBody;
                }
                catch (Exception e)
                {
                    logMessage.AddMessageToLogChain($"An exception has occured returning default value of {typeof(TBody)}", e, e.StackTrace);
                    return default;
                }
                finally
                {
                    if (typeof(TBody) != typeof(LogMessage)) _logger.SendMessage(logMessage);
                }
            }
        }
    }
}
