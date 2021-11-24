using ParkingAds.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageModel
{
    public class HistoryMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CorrelationId { get; set; } = null;
        public List<History> History { get; set; } = new();

        public void AddHistory(string queueName, string messageBrokerName, string payload = "", DateTime? timestamp = null)
        {
            History.Add(new History()
            {
                QueueName = queueName,
                MessageBrokerName = messageBrokerName,
                Payload = payload,
                Timestamp = timestamp ?? DateTime.Now
            });
        }
    }
}
