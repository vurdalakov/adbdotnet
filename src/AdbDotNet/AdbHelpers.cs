namespace Vurdalakov
{
    using System;
    using System.IO;

    public static class AdbHelpers
    {
        public static String CombinePath(String path1, String path2)
        {
            return Path.Combine(path1, path2).Replace('\\', '/');
        }

        private static DateTime GetUnixEpoch()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime FromUnixTime(Int32 unixTime)
        {
            return GetUnixEpoch().AddSeconds(unixTime).ToLocalTime();
        }

        public static Int32 ToUnixTime(DateTime dateTime)
        {
            var timeSpan = dateTime.ToUniversalTime() - GetUnixEpoch();
            return Convert.ToInt32(timeSpan.TotalSeconds);
        }
    }
}
