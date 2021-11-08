using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Interfaces
{
    public interface IProducer<TBody>
    {
        public void SendMessage(TBody input);
    }
}
