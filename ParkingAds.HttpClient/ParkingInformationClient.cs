using Newtonsoft.Json;
using ParkingAds.HttpClient.Bases;
using ParkingAds.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.HttpClient
{
    public class ParkingInformationClient : BaseHttpClient
    {
        private readonly string PARKING_URL;

        public ParkingInformationClient()
        {
            PARKING_URL = ConfigurationManager.AppSettings["ParkingInformationURL"];
        }

        public List<ParkingInformation> GetParkingInformations()
        {
            Random rand = new();
            return new List<ParkingInformation>()
            {
                new ParkingInformation()
                {
                    Coord = "123x123",
                    Current = rand.Next(0, 1000),
                    Date = DateTime.Now.ToString(),
                    Max = 10000,
                    Name = "Aalborg by"
                },
                new ParkingInformation()
                {
                    Coord = "333x333",
                    Current = rand.Next(0, 100),
                    Date = DateTime.Now.ToString(),
                    Max = 1000,
                    Name = "Struer by"
                },
                new ParkingInformation()
                {
                    Coord = "999x999",
                    Current = rand.Next(0, 500),
                    Date = DateTime.Now.ToString(),
                    Max = 5000,
                    Name = "Holstebro by"
                },
                new ParkingInformation()
                {
                    Coord = "876x345",
                    Current = rand.Next(0, 10000),
                    Date = DateTime.Now.ToString(),
                    Max = 100000,
                    Name = "København by"
                }
            };

            var parkingInfo = Get(PARKING_URL);
            if (parkingInfo.StatusCode != HttpStatusCode.OK) return new();
            List<ParkingInformation> parkingInformation = JsonConvert.DeserializeObject<List<ParkingInformation>>(parkingInfo.Content);
            return parkingInformation;
        }
    }
}
