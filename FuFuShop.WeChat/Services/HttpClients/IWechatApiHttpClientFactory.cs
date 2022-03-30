using SKIT.FlurlHttpClient.Wechat.Api;

namespace FuFuShop.WeChat.Services.HttpClients
{
    public interface IWeChatApiHttpClientFactory
    {
        /// <summary>
        /// 微信公众号请求
        /// </summary>
        /// <returns></returns>
        WechatApiClient CreateWeXinClient();

        /// <summary>
        /// 微信小程序请求
        /// </summary>
        /// <returns></returns>
        WechatApiClient CreateWxOpenClient();
    }
}
