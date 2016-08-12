using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChristianMoser.WpfInspector.Services
{
    public enum LogSeverity
    {
        Info,
        Warning,
        Error
    }

    public class LoggerService
    {
        public void Log( LogSeverity severity, Exception exception)
        {
            
        }
    }
}
