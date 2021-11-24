using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageModel;
using ParkingAds.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Producers
{
    public class ParkingInformationProducer : BaseProducer<ParkingInformationMessage>
    {
        private readonly string _queueDestination;

        public ParkingInformationProducer(string queueDestination) : base(Resx.QueueNames.Wiretap, true, false, false, null)
        {
            _queueDestination = queueDestination;
        }

        public ParkingInformationMessage CreateParkingInformationMessage(ParkingInformation parkingInformation)
        {
            ParkingInformationMessage message = new();
            message.ParkingInformation = parkingInformation;
            message.WiretapMessage.QueueDestination = _queueDestination;
            message.History.AddHistory(_queueDestination, nameof(ParkingInformationProducer));
            return message;
        }
    }
}
