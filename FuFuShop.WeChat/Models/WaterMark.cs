using FuFuShop.WeChat.Utilities;

namespace FuFuShop.WeChat.Models
{
    /// <summary>
    /// 水印
    /// </summary>
    [Serializable]
    public class Watermark
    {
        public string appid { get; set; }
        public long timestamp { get; set; }

        public DateTimeOffset DateTimeStamp
        {
            get { return DateTimeHelper.GetDateTimeFromXml(timestamp); }
        }
    }
}
