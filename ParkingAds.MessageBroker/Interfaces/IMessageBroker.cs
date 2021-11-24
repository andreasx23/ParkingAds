using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Interfaces
{
    public interface IMessageBroker
    {
        public void TryCreateQueue(IModel channel, string queueName = "");
    }
}
