using log4net;

namespace Utils
{
    public static class LogUtils
    {
        private static ILog log = LogManager.GetLogger("TextInfo");

        public static void Error(string s)
        {
            log.Error(s);
        }

        public static void Error(string format, params object[] args)
        {
            log.Error(string.Format(format, args));
        }
    }
}
