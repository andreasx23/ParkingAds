using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageModel
{
    public class WiretapMessage
    {
        public Guid Id { get; set; }
        public Guid? CorrelationId { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string SendToQueue { get; set; }
        public string RecieveFromQueue { get; set; }
        public string Type { get; set; } = "*";

        public void WriteToFile(string payload) //TODO IMPLEMENT WRITE TO FILE
        {
            Console.WriteLine("Simulating writing to file..");
            Console.WriteLine(payload);
        }
    }
}
