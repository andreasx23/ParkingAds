using ParkingAds.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageModel
{
    public class ParkingInformationMessage
    {
        public ParkingInformation ParkingInformation { get; set; } = new();
        public HistoryMessage History { get; set; } = new();
        public WiretapMessage WiretapMessage { get; set; } = new();
    }
}
