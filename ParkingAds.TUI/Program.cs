using Newtonsoft.Json;
using NLog;
using ParkingAds.HttpClient;
using ParkingAds.MessageBroker;
using ParkingAds.MessageBroker.Consumers;
using ParkingAds.MessageBroker.Producers;
using ParkingAds.MessageModel;
using ParkingAds.Model;
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
            const string queueName = "ParkingInformation";

            //Customers - ParkingInformation producers
            int customers = 1;
            for (int i = 0; i < customers; i++)
            {
                new Thread(() =>
                {
                    ParkingInformationProducer producer = new(queueName);
                    while (true)
                    {
                        var parkingInfos = _parkingInformationClient.GetParkingInformations();
                        ParkingInformation info = parkingInfos[rand.Next(0, parkingInfos.Count)];
                        string ad = _adClient.GetAd();
                        info.HttpEncodedAd = ad;
                        ParkingInformationMessage message = producer.CreateParkingInformationMessage(info);
                        producer.SendMessage(message);

                        //if (parkingInfos.Count > 0)
                        //{
                        //    for (int i = 0; i < rand.Next(0, 5) + 1; i++)
                        //    {
                        //        var ad = _adClient.GetAd();
                        //        ParkingInformation info = parkingInfos[rand.Next(0, parkingInfos.Count)];
                        //        info.HttpEncodedAd = ad;
                        //        ParkingInformationMessage message = producer.CreateParkingInformationMessage(info);
                        //        producer.SendMessage(message);
                        //    }
                        //}
                        //Thread.Sleep(rand.Next(0, 2500));
                    }
                })
                { IsBackground = true }.Start();
            }

            /*
             * Wiretap consumer
             * Consumes the message and sends to another queue. If EnableWiretap is true it will print the message acting like were doing wiretap
            */
            int wiretapConsumers = 4;
            for (int i = 0; i < wiretapConsumers; i++)
            {
                new Thread(() =>
                {
                    WiretapConsumer consumer = new();
                    while (true)
                    {
                        consumer.ConsumeWiretapMessages();
                    }
                })
                { IsBackground = true }.Start();
            }

            //ParkingInformation consumers
            int parkingInformationConsumers = 5;
            for (int i = 0; i < parkingInformationConsumers; i++)
            {
                new Thread(() =>
                {
                    ParkingInformationConsumer consumer = new(queueName);
                    int id = i + 1;
                    while (true)
                    {
                        var message = consumer.ConsumeMessage();
                        if (message != null)
                        {
                            Console.WriteLine($"Id: {id} says {message.ParkingInformation}");
                            //Thread.Sleep(rand.Next(0, 250));
                        }
                    }
                })
                { IsBackground = true }.Start();
            }

            //Logging consumers
            int loggingConsumers = 10;
            for (int i = 0; i < loggingConsumers; i++)
            {
                new Thread(() =>
                {
                    LogConsumer consumer = new();
                    while (true)
                    {
                        consumer.ConsumeLogs();
                    }
                })
                { IsBackground = true }.Start();
            }

            Console.WriteLine("Running");
            Console.ReadLine();
        }
    }
}
