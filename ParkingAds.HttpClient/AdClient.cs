using ParkingAds.HttpClient.Bases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.HttpClient
{
    public class AdClient : BaseHttpClient
    {
        private readonly string AD_URL;

        public AdClient()
        {
            AD_URL = ConfigurationManager.AppSettings["ParkingAdURL"];
        }

        public string GetAd()
        {
            var ad = Get(AD_URL);
            return ad.StatusCode == HttpStatusCode.OK ? ad.Content : string.Empty;
        }

        public string GetAdMockData()
        {
            string randomAd = string.Empty;
            Random rand = new();
            for (int i = 0; i < 10; i++)
            {
                char value = (char)rand.Next('A', 'Z');
                randomAd += value;
            }
            return randomAd;
        }
    }
}
