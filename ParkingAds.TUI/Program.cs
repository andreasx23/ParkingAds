using Newtonsoft.Json;
using NLog;
using ParkingAds.MessageBroker;
using ParkingAds.MessageBroker.Consumers;
using ParkingAds.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace ParkingAds.TUI
{
    public class Program
    {
        private static readonly RestClient _client = new();

        private static IRestResponse Get(string uri)
        {
            IRestRequest request = new RestRequest($"{uri}", Method.GET);
            IRestResponse response = _client.Execute(request);
            return response;
        }

        private static string GetAd()
        {
            var ad = Get("http://psuaddservice.fenris.ucn.dk/");
            return ad.StatusCode == HttpStatusCode.OK ? ad.Content : string.Empty;
        }

        private static List<ParkingInformation> GetParkingInformations()
        {
            var parkingInfo = Get("http://psuparkingservice.fenris.ucn.dk/service");
            if (parkingInfo.StatusCode != HttpStatusCode.OK) return new();
            List<ParkingInformation> parkingInformation = JsonConvert.DeserializeObject<List<ParkingInformation>>(parkingInfo.Content);
            return parkingInformation;
        }

        public static void Main(string[] args)
        {
            Random rand = new();
            const string queueName = "ParkingInformation";
            new Thread(() =>
            {
                while (true)
                {
                    var parkingInfos = GetParkingInformations();                    
                    if (parkingInfos.Count > 0)
                    {
                        Producer<ParkingInformation> producer = new(queueName);
                        var ad = GetAd();
                        for (int i = 0; i < rand.Next(0, 5) + 1; i++)
                        {
                            var info = parkingInfos[rand.Next(0, parkingInfos.Count)];
                            info.HttpEncodedAd = ad;
                            producer.SendMessage(info);
                        }
                    }
                    Thread.Sleep(rand.Next(0, 1000));
                }
            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    Consumer<ParkingInformation> c = new(queueName);
                    var message = c.ConsumeMessageWithPolling();
                    Console.WriteLine(message);
                    Thread.Sleep(rand.Next(0, 250));
                }
            })
            { IsBackground = true }.Start();

            for (int i = 0; i < 3; i++)
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        LogConsumer log = new();
                        log.ConsumeLogs();
                    }
                })
                { IsBackground = true }.Start();
            }

            Console.WriteLine("Running");
            Console.ReadLine();
        }
    }
}
