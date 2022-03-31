using CoreCms.Net.IServices;
using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 订单调用接口数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpContextUser _user;
        private readonly IOrderServices _orderServices;
        private readonly IAreaServices _areaServices;
        private readonly IBillDeliveryServices _billDeliveryServices;
        private readonly ILogisticsServices _logisticsServices;
        private readonly IGoodsServices _goodsServices;
        private readonly IStoreServices _storeServices;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderController(IHttpContextUser user
        , IOrderServices orderServices
        , IBillAftersalesServices aftersalesServices
        , ISettingServices settingServices
        , IAreaServices areaServices
        , IBillReshipServices reshipServices, IShipServices shipServices
        , IBillDeliveryServices billDeliveryServices, ILogisticsServices logisticsServices, IGoodsServices goodsServices, IStoreServices storeServices)
        {
            _user = user;
            _orderServices = orderServices;
            _aftersalesServices = aftersalesServices;
            _settingServices = settingServices;
            _areaServices = areaServices;
            _reshipServices = reshipServices;
            _shipServices = shipServices;
            _billDeliveryServices = billDeliveryServices;
            _logisticsServices = logisticsServices;
            _goodsServices = goodsServices;
            _storeServices = storeServices;
        }




    }
}