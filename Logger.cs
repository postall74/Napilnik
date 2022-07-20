using System;
using System.Collections.Generic;
using System.IO;

namespace Logger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Pathfinder logConsole = new Pathfinder(new ConsoleLogger());
            Pathfinder logFile = new Pathfinder(new FileLogger());
            Pathfinder logConsoleFriday = new Pathfinder(new FridayLogger(new ConsoleLogger()));
            Pathfinder logFileFriday = new Pathfinder(new FridayLogger(new FileLogger()));
            ConsistentLogging collection = new ConsistentLogging(new List<ILogger>());
            Pathfinder logConsoleAndFileFriday = new Pathfinder(collection.Create(new ConsoleLogger(),new FridayLogger(new FileLogger())));
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            File.WriteAllText("log.txt", message);
        }
    }

    public class FridayLogger : ILogger
    {
        private readonly ILogger _logger;

        public FridayLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                _logger.Log(message);
            }
        }
    }

    public class ConsistentLogging : ILogger
    {
        private readonly IEnumerable<ILogger> _loggers;

        public ConsistentLogging(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public ConsistentLogging Create(params ILogger[] loggers)
        {
            return new ConsistentLogging(loggers);
        }

        public void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message);
            }
        }
    }

    public class Pathfinder
    {
        private readonly ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            _logger = logger;
        }
    }
}
