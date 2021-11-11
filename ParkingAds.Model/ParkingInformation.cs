using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.Model
{
    public class ParkingInformation
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Coord { get; set; }
        public int Max { get; set; }
        public int Current { get; set; }
        public string HttpEncodedAd { get; set; }

        public override string ToString()
        {
            return $"{Name} har {Max - Current} ledige pladser ud af {Max}. Information opdateret: {Date}{Environment.NewLine}{HttpEncodedAd}";
        }
    }
}
