namespace FuFuShop.WeChat.Models
{
    /// <summary>
    /// 用户绑定手机号解密类
    /// </summary>
    public class DecodedPhoneNumber : DecodeEntityBase
    {
        /// <summary>
        /// 用户绑定的手机号（国外手机号会有区号）
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// 没有区号的手机号
        /// </summary>
        public string purePhoneNumber { get; set; }
        /// <summary>
        /// 区号（Senparc注：国别号）
        /// </summary>
        public string countryCode { get; set; }
    }
}
