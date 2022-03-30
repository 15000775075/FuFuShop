

using Newtonsoft.Json.Linq;

namespace FuFuShop.WeChat.Models
{
    /// <summary>
    ///     处理器-微信模板消息【小程序，公众号都走这里】
    /// </summary>
    public class SendWxTemplateMessage
    {
        /// <summary>
        ///     用户序列
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        ///     类型
        /// </summary>
        public string code { get; set; }

        /// <summary>
        ///     传递数据
        /// </summary>
        public JObject parameters { get; set; }
    }
}