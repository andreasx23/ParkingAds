using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.HttpClient.Bases
{
    public abstract class BaseHttpClient
    {
        private readonly RestClient _client = new();

        protected virtual IRestResponse Get(string uri)
        {
            IRestRequest request = new RestRequest($"{uri}", Method.GET);
            IRestResponse response = _client.Execute(request);
            return response;
        }
    }
}
