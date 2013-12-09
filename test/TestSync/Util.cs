using log4net;

namespace TestSync
{
    public class Util
    {
        private static readonly ILog log = LogManager.GetLogger(System.Diagnostics.Debugger.IsAttached ? "Debug" : "Product");
        public static ILog GetLog()
        {
            return log;
        }
    }
}