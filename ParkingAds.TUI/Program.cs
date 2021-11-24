using Newtonsoft.Json;
using NLog;
using ParkingAds.HttpClient;
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
        private static readonly AdClient _adClient = new();
        private static readonly ParkingInformationClient _parkingInformationClient = new();

        public static void Main(string[] args)
        {
            Random rand = new();
            const string queueName = "ParkingInformation";
            new Thread(() =>
            {
                while (true)
                {
                    var parkingInfos = _parkingInformationClient.GetParkingInformations();                  
                    if (parkingInfos.Count > 0)
                    {
                        Producer<ParkingInformation> producer = new(queueName);
                        var ad = _adClient.GetAd();
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
