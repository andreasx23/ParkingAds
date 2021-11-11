using NLog;
using ParkingAds.MessageBroker.Bases;
using ParkingAds.MessageBroker.Resx;
using ParkingAds.Model;
using ParkingAds.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingAds.MessageBroker.Consumers
{
    public class LogConsumer : BaseConsumer<LogMessage>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Array _logTypes;

        public LogConsumer() : base(QueueNames.InfoLog, true, false, false, null)
        {
            _logTypes = Enum.GetValues(typeof(LogType));
        }

        public void ConsumeLogs()
        {
            foreach (LogType type in _logTypes)
            {
                LogMessage logMessage;
                switch (type)
                {
                    case LogType.DEBUG:
                        QueueName = QueueNames.DebugLog;
                        logMessage = ConsumeMessage();
                        if (logMessage != null)
                        {
                            _logger.Debug(logMessage.Exception, logMessage.Message);
                            if (!string.IsNullOrEmpty(logMessage.StackTrace)) _logger.Debug(logMessage.StackTrace);
                        }
                        break;
                    case LogType.INFO:
                        QueueName = QueueNames.InfoLog;
                        logMessage = ConsumeMessage();
                        if (logMessage != null) _logger.Info(logMessage.Exception, logMessage.Message);
                        break;
                    case LogType.ERROR:
                        QueueName = QueueNames.ErrorLog;
                        logMessage = ConsumeMessage();
                        if (logMessage != null)
                        {
                            _logger.Error(logMessage.Exception, logMessage.Message);
                            if (!string.IsNullOrEmpty(logMessage.StackTrace)) _logger.Error(logMessage.StackTrace);
                        }
                        break;
                    case LogType.WARN:
                        QueueName = QueueNames.WarnLog;
                        logMessage = ConsumeMessage();
                        if (logMessage != null)
                        {
                            _logger.Warn(logMessage.Exception, logMessage.Message);
                            if (!string.IsNullOrEmpty(logMessage.StackTrace)) _logger.Warn(logMessage.StackTrace);
                        }
                        break;
                    case LogType.TRACE:
                        QueueName = QueueNames.TraceLog;
                        logMessage = ConsumeMessage();
                        if (logMessage != null)
                        {
                            _logger.Trace(logMessage.Exception, logMessage.Message);
                            if (!string.IsNullOrEmpty(logMessage.StackTrace)) _logger.Trace(logMessage.StackTrace);
                        }
                        break;
                }
            }
        }
    }
}
