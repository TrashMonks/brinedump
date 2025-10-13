using System;

namespace TrashMonks.Brinedump
{
    public static partial class Static
    {
        private static SimpleFileLogger BDLogger;
        private static object LogLock = new Object();
        public static void BDLog(string message, Exception e = null)
        {
            lock (LogLock)
            {
                if (BDLogger == null) BDLogger = new SimpleFileLogger("Brinedump.log");

                if (e != null) BDLogger.Error(message + "\n" + e.ToString());
                else BDLogger.Info(message);
            }
            ;
        }
    }
}
