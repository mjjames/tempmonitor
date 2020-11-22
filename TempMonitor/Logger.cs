using System;

namespace TempMonitor
{
    internal class Logger
    {
        public void LogMessage(Func<string> message)
        {
#if DEBUG
            Console.WriteLine(message());
#endif
        }
    }
}
