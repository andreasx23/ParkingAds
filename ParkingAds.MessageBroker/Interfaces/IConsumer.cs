using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Interfaces
{
    public interface IConsumer<TBody>
    {
        public TBody ConsumeMessageWithPolling();
        public TBody ConsumeMessage();

    }
}
