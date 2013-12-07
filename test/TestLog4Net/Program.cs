using System;
using log4net;

namespace TestLog4Net
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Diagnostics.Debugger.IsAttached ? "Debug" : "Product");

        static void Main(string[] args)
        {
            Log.Debug("Log Debug");
            Log.Info("Log Info");
            Log.Warn("Log Warn");
            Log.Error("Log Error");
            Console.ReadLine();
        }
    }
}
