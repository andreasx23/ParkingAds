using Newtonsoft.Json;
using ParkingAds.HttpClient.Bases;
using ParkingAds.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.HttpClient
{
    public class ParkingInformationClient : BaseHttpClient
    {
        private const string PARKING_URL = "http://psuparkingservice.fenris.ucn.dk/service";

        public List<ParkingInformation> GetParkingInformations()
        {
            var parkingInfo = Get(PARKING_URL);
            if (parkingInfo.StatusCode != HttpStatusCode.OK) return new();
            List<ParkingInformation> parkingInformation = JsonConvert.DeserializeObject<List<ParkingInformation>>(parkingInfo.Content);
            return parkingInformation;
        }
    }
}
