using ParkingAds.HttpClient.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.HttpClient
{
    public class AdClient : BaseHttpClient
    {
        private const string AD_URL = "http://psuaddservice.fenris.ucn.dk/";

        public string GetAd()
        {
            var ad = Get(AD_URL);
            return ad.StatusCode == HttpStatusCode.OK ? ad.Content : string.Empty;
        }
    }
}
