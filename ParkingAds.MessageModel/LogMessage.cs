using ParkingAds.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkingAds.MessageModel
{
    public class LogMessage
    {
        public List<LogMessage> _logChain { get; set; } = new();

        public Guid Id { get; set; }
        public Exception Exception { get; set; }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (CorrelationId.HasValue)
                    _message = $"[{CorrelationId}]{value}";
                else
                    _message = value;
            }
        }
        public string StackTrace { get; set; }
        public Guid? CorrelationId { get; set; } = null;
        public LogType LogType { get; set; }

        public LogMessage(string message, Guid? correlationId = null)
        {
            CorrelationId = correlationId;
            Message = message;
            LogType = LogType.INFO;
        }

        public LogMessage(string message, LogType logType, Guid? correlationId = null)
        {
            CorrelationId = correlationId;
            Message = message;
            LogType = logType;
        }

        public LogMessage(string message, Exception exception, string stackTrace, Guid? correlationId = null)
        {
            CorrelationId = correlationId;
            Message = message;
            Exception = exception;
            StackTrace = stackTrace;
            LogType = LogType.ERROR;
        }

        public LogMessage() //TODO HANDLE JsonConstructor so empty constructor is no longer required for deserialize
        {

        }

        //[JsonConstructor]
        //public LogMessage(Guid id, Exception exception, string message, string stackTrace, Guid? correlationId, LogType logType)
        //{
        //    Id = id;
        //    Message = message;
        //    Exception = exception;
        //    StackTrace = stackTrace;
        //    LogType = logType;
        //    //_logChain = logChain;
        //    CorrelationId = correlationId;
        //}

        public void AddMessageToLogChain(string message)
        {
            _logChain.Add(new LogMessage(message, CorrelationId ?? null));
        }

        public void AddMessageToLogChain(string message, LogType logType)
        {
            _logChain.Add(new LogMessage(message, logType, CorrelationId ?? null));
        }

        public void AddMessageToLogChain(string message, Exception exception, string stackTrace)
        {
            _logChain.Add(new LogMessage(message, exception, stackTrace, CorrelationId ?? null));
        }

        public List<LogMessage> GetLogChain()
        {
            return _logChain;
        }

        public static Guid GenerateCorrelationId()
        {
            return Guid.NewGuid();
        }
    }
}
