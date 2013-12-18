using log4net;

namespace ZTask
{
    public class Util
    {
        public static readonly ILog Log = LogManager.GetLogger(System.Diagnostics.Debugger.IsAttached ? "Debug" : "Product");
    }
}