﻿using NLog;
using ParkingAds.MessageBroker;
using ParkingAds.MessageBroker.Consumers;
using System;
using System.Threading;

namespace ParkingAds.TUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //for (int i = 0; i < 200; i++)
            //{
            //    LogConsumer log = new();
            //    log.ConsumeLogs();
            //}

            Random rand = new();
            const string queueName = "123";
            new Thread(() =>
            {
                while (true)
                {
                    Producer<string> d = new(queueName);
                    d.SendMessage("Test123");
                    Thread.Sleep(rand.Next(0, 1000));
                }
            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    Consumer<string> c = new(queueName);
                    Console.WriteLine(c.ConsumeMessageWithPolling());
                    Thread.Sleep(rand.Next(0, 250));
                }

            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    LogConsumer log = new();
                    log.ConsumeLogs();
                }

            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    LogConsumer log = new();
                    log.ConsumeLogs();
                }

            })
            { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    LogConsumer log = new();
                    log.ConsumeLogs();
                }

            })
            { IsBackground = true }.Start();

            Console.WriteLine("Running");
            Console.ReadLine();
        }
    }
}