namespace FuFuShop.WeChat.Utilities
{
    /// <summary>微信日期处理帮助类</summary>
    public class DateTimeHelper
    {
        /// <summary>Unix起始时间</summary>
        public static readonly DateTimeOffset BaseTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>转换微信DateTime时间到C#时间</summary>
        /// <param name="dateTimeFromXml">微信DateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromXml(long dateTimeFromXml) => GetDateTimeOffsetFromXml(dateTimeFromXml).LocalDateTime;

        /// <summary>转换微信DateTime时间到C#时间</summary>
        /// <param name="dateTimeFromXml">微信DateTime</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromXml(string dateTimeFromXml) => GetDateTimeFromXml(long.Parse(dateTimeFromXml));

        /// <summary>转换微信DateTimeOffset时间到C#时间</summary>
        /// <param name="dateTimeFromXml">微信DateTime</param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeOffsetFromXml(long dateTimeFromXml) => BaseTime.AddSeconds(dateTimeFromXml).ToLocalTime();

        /// <summary>转换微信DateTimeOffset时间到C#时间</summary>
        /// <param name="dateTimeFromXml">微信DateTime</param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeOffsetFromXml(string dateTimeFromXml) => (DateTimeOffset)GetDateTimeFromXml(long.Parse(dateTimeFromXml));

        /// <summary>获取微信DateTime（UNIX时间戳）</summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        [Obsolete("请使用 GetUnixDateTime(dateTime) 方法")]
        public static long GetWeixinDateTime(DateTime dateTime) => GetUnixDateTime(dateTime);

        /// <summary>获取Unix时间戳</summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetUnixDateTime(DateTimeOffset dateTime) => (long)(dateTime - BaseTime).TotalSeconds;

        /// <summary>获取Unix时间戳</summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetUnixDateTime(DateTime dateTime) => (long)((DateTimeOffset)dateTime.ToUniversalTime() - BaseTime).TotalSeconds;
    }
}
