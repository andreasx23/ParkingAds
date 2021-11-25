using Newtonsoft.Json;
using NLog;
using ParkingAds.HttpClient;
using ParkingAds.MessageBroker;
using ParkingAds.MessageBroker.Consumers;
using ParkingAds.MessageBroker.Producers;
using ParkingAds.MessageModel;
using ParkingAds.Model;
using ParkingAds.Singleton;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace ParkingAds.TUI
{
    public class Program
    {
        private static readonly AdClient _adClient = new();
        private static readonly ParkingInformationClient _parkingInformationClient = new();

        public static void Main(string[] args)
        {
            Random rand = new();
            const string QUEUE_NAME = "ParkingInformation";
            const string AD = "AD";

            //Redis caching for ad
            int redisThreads = 1;
            for (int i = 0; i < redisThreads; i++)
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        string ad = _adClient.GetAdMockData();
                        if (!string.IsNullOrEmpty(ad) && !ad.ToLower().Contains("something bad happened"))
                            RedisSingleton.Instance.GetDatabase().StringSet(AD, ad);
                        Thread.Sleep(2000);
                    }
                })
                { IsBackground = true }.Start();
            }

            //Customers - ParkingInformation producers
            int customers = 1;
            for (int i = 0; i < customers; i++)
            {
                new Thread(() =>
                {
                    ParkingInformationProducer producer = new(QUEUE_NAME);
                    while (true)
                    {
                        List<ParkingInformation> parkingInfos = _parkingInformationClient.GetParkingInformationsMockData();
                        if (parkingInfos.Count > 0)
                        {
                            ParkingInformation info = parkingInfos[rand.Next(0, parkingInfos.Count)];
                            info.HttpEncodedAd = RedisSingleton.Instance.GetDatabase().StringGet(AD);
                            ParkingInformationMessage message = producer.CreateParkingInformationMessage(info);
                            producer.SendMessage(message);
                        }
                        Thread.Sleep(rand.Next(0, 500));
                    }
                })
                { IsBackground = true }.Start();
            }

            /*
             * Wiretap consumer
             * Consumes the message and sends to another queue. If EnableWiretap is true it will print the message acting like were doing wiretap
            */
            int wiretapConsumers = 1;
            for (int i = 0; i < wiretapConsumers; i++)
            {
                new Thread(() =>
                {
                    WiretapConsumer consumer = new();
                    while (true)
                    {
                        consumer.ConsumeWiretapMessages();
                        Thread.Sleep(rand.Next(0, 50));
                    }
                })
                { IsBackground = true }.Start();
            }

            //ParkingInformation consumers
            int parkingInformationConsumers = 1;
            for (int i = 0; i < parkingInformationConsumers; i++)
            {
                new Thread(() =>
                {
                    ParkingInformationConsumer consumer = new(QUEUE_NAME);
                    int id = i + 1;
                    while (true)
                    {
                        var message = consumer.ConsumeMessage();
                        if (message != null)
                            Console.WriteLine($"Id: {id} says {message.ParkingInformation}");
                        Thread.Sleep(rand.Next(0, 100));
                    }
                })
                { IsBackground = true }.Start();
            }

            //Logging consumers
            int loggingConsumers = 1;
            for (int i = 0; i < loggingConsumers; i++)
            {
                new Thread(() =>
                {
                    LogConsumer consumer = new();
                    while (true)
                    {
                        consumer.ConsumeLogs();
                        Thread.Sleep(rand.Next(0, 25));
                    }
                })
                { IsBackground = true }.Start();
            }

            Console.WriteLine("Running");
            Console.ReadLine();
        }
    }
}
