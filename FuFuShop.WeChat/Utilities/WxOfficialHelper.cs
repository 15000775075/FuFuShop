namespace FuFuShop.WeChat.Utilities
{
    /// <summary>
    /// 微信公众号帮助类
    /// </summary>
    public static class WxOfficialHelper
    {

        public static string geturl(string url, string weXinAppId, int scope = 1)
        {

            return "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + weXinAppId + "&redirect_uri=" + url + "&response_type=code&scope=" + scope + "&state=jshop#wechat_redirect";
        }

    }
}
