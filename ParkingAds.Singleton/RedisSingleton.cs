using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.Singleton
{
    public sealed class RedisSingleton
    {
        //https://medium.com/idomongodb/installing-redis-server-using-docker-container-453c3cfffbdf
        private static readonly object padlock = new();
        private static ConnectionMultiplexer instance = null;
        private static readonly string HOSTNAME;
        private static readonly string PORT;
        
        static RedisSingleton()
        {
            HOSTNAME = ConfigurationManager.AppSettings["HostName"];
            PORT = ConfigurationManager.AppSettings["Port"];
        }

        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = ConnectionMultiplexer.Connect(new ConfigurationOptions
                    {
                        EndPoints = { $"{HOSTNAME}:{PORT}" }
                    });
                }
                return instance;

                //lock (padlock)
                //{
                //    if (instance == null)
                //    {
                //        instance = ConnectionMultiplexer.Connect(new ConfigurationOptions
                //        {
                //            EndPoints = { $"{HOSTNAME}:{PORT}" }
                //        });
                //    }
                //    return instance;
                //}
            }
        }
    }
}
