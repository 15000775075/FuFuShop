
using FuFuShop.Common.AppSettings;
using FuFuShop.Common.Caching;
using FuFuShop.Common.Loging;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using FuFuShop.Services.Shop;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;


namespace FuFuShop.Services
{
    /// <summary>
    /// 消息配置表 接口实现
    /// </summary>
    public class MessageCenterServices : BaseServices<MessageCenter>, IMessageCenterServices
    {
        private readonly IMessageCenterRepository _dal;

        private readonly IServiceProvider _serviceProvider;
        private readonly IRedisOperationRepository _redisOperationRepository;

        private readonly IUnitOfWork _unitOfWork;
        public MessageCenterServices(IUnitOfWork unitOfWork, IMessageCenterRepository dal, IServiceProvider serviceProvider, ISysTaskLogServices taskLogServices, IRedisOperationRepository redisOperationRepository)
        {
            _dal = dal;
            _serviceProvider = serviceProvider;
            _redisOperationRepository = redisOperationRepository;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// 商家发送信息助手
        /// </summary>
        /// <param name="userId">接受者id</param>
        /// <param name="code">模板编码</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public async Task<WebApiCallBack> SendMessage(int userId, string code, JObject parameters)
        {
            try
            {
                var jm = new WebApiCallBack();

                using var container = _serviceProvider.CreateScope();

                var userServices = container.ServiceProvider.GetService<IUserServices>();
                var settingServices = container.ServiceProvider.GetService<ISettingServices>();
                var smsServices = container.ServiceProvider.GetService<ISmsServices>();
                var allConfigs = await settingServices.GetConfigDictionaries();
                var messageServices = container.ServiceProvider.GetService<IMessageServices>();

                var config = await _dal.QueryByClauseAsync(p => p.code == code);
                if (config == null)
                {
                    jm.msg = GlobalErrorCodeVars.Code10100;
                    return jm;
                }

                //站内消息
                if (config.isMessage && code != GlobalEnumVars.PlatformMessageTypes.SellerOrderNotice.ToString())
                {
                    await messageServices.Send(userId, code, parameters);
                }
                //微信模板消息【小程序，公众号都走这里】
                if (config.isWxTempletMessage &&
                    (code == GlobalEnumVars.PlatformMessageTypes.OrderPayed.ToString() || code == GlobalEnumVars.PlatformMessageTypes.DeliveryNotice.ToString() || code == GlobalEnumVars.PlatformMessageTypes.RemindOrderPay.ToString()))
                {
                    var @params = new JObject();
                    @params.Add("parameters", parameters);

                    var data = new
                    {
                        userId,
                        code,
                        parameters = @params
                    };

                    //队列推送消息
                    await _redisOperationRepository.ListLeftPushAsync(RedisMessageQueueKey.SendWxTemplateMessage, JsonConvert.SerializeObject(data));
                }
                jm.status = true;
                return jm;
            }
            catch (Exception ex)
            {
                NLogUtil.WriteAll(LogLevel.Trace, LogType.RefundResultNotification, "商家发送信息助手", JsonConvert.SerializeObject(ex));
                throw;
            }
        }

    }
}
