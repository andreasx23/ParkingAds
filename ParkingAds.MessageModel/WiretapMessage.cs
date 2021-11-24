using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageModel
{
    public class WiretapMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CorrelationId { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string QueueDestination { get; set; } //Where are we going?
        public string Type { get; set; } = "*";
    }
}
