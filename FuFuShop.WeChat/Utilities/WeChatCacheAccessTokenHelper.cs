using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching.Manual;
using FuFuShop.Model.Entities;

namespace FuFuShop.WeChat.Utilities
{
    /// <summary>
    /// 微信帮助类
    /// </summary>
    public static class WeChatCacheAccessTokenHelper
    {
        /// <summary>
        /// 获取微信小程序accessToken
        /// </summary>
        /// <returns></returns>
        public static string GetWxOpenAccessToken()
        {
            //获取小程序AccessToken
            var cacheAccessToken = ManualDataCache.Instance.Get<WeChatAccessToken>(GlobalEnumVars.AccessTokenEnum.WxOpenAccessToken.ToString());
            return cacheAccessToken?.accessToken;
        }

        /// <summary>
        /// 获取微信公众号accessToken
        /// </summary>
        /// <returns></returns>
        public static string GetWeChatAccessToken()
        {
            //获取微信AccessToken
            var cacheAccessToken = ManualDataCache.Instance.Get<WeChatAccessToken>(GlobalEnumVars.AccessTokenEnum.WeiXinAccessToken.ToString());
            return cacheAccessToken?.accessToken;
        }

    }
}
