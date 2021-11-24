using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Consumers
{
    public class ParkingInformationConsumer : BaseConsumer<ParkingInformationMessage>
    {
        public ParkingInformationConsumer(string queueName, bool isDurable = true, bool isExclusive = false, bool shouldAutoDelete = false, IDictionary<string, object> queueArguments = null) : base(queueName, isDurable, isExclusive, shouldAutoDelete, queueArguments)
        {
        }
    }
}
