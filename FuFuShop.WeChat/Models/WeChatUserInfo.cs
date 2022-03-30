namespace FuFuShop.WeChat.Models
{
    /// <summary>
    /// 微信小程序用户信息结构
    /// </summary>

    public class WeChatUserInfo

    {
        public string openId { get; set; }

        public string nickName { get; set; }

        public int gender { get; set; }

        public string city { get; set; }

        public string province { get; set; }

        public string country { get; set; }

        public string avatarUrl { get; set; }

        public string unionId { get; set; }

        public Watermark watermark { get; set; }


    }


    [Serializable]
    public class DecodeEntityBase
    {
        public Watermark watermark { get; set; }
    }

    /// <summary>
    /// 解码后的用户信息
    /// </summary>
    [Serializable]
    public class DecodedUserInfo : DecodeEntityBase
    {
        public string openId { get; set; }
        public string nickName { get; set; }
        public int gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string avatarUrl { get; set; }
        public string unionId { get; set; }
    }

}
