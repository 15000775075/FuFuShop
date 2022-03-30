
namespace FuFuShop.WeChat.Models
{
    /// <summary>
    ///     微信接口回调Json实体
    /// </summary>
    public class WeChatApiCallBack
    {
        /// <summary>
        ///     提交数据
        /// </summary>
        public object OtherData { get; set; } = null;

        /// <summary>
        ///     状态码
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        ///     信息说明。
        /// </summary>
        public string Msg { get; set; } = "响应成功";

        /// <summary>
        ///     返回数据
        /// </summary>
        public string Data { get; set; } = "success";
    }
}