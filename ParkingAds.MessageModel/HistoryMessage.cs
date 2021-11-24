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
        public Guid Id { get; set; }
        public Guid? CorrelationId { get; set; } = null;
        public List<History> History { get; set; } = new();
    }
}
