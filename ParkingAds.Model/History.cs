using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.Model
{
    public class History
    {
        public string QueueName { get; set; }
        public string MessageBrokerName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Payload { get; set; }
    }
}
