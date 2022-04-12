
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using Newtonsoft.Json.Linq;

namespace FuFuShop.Services
{
    /// <summary>
    ///     消息配置表 服务工厂接口
    /// </summary>
    public interface IMessageCenterServices : IBaseServices<MessageCenter>
    {
        /// <summary>
        ///     商家发送信息助手
        /// </summary>
        /// <param name="userId">接受者id</param>
        /// <param name="code">模板编码</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<WebApiCallBack> SendMessage(int userId, string code, JObject parameters);
    }
}