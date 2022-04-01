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


        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderController(IHttpContextUser user
        , IOrderServices orderServices
        , IAreaServices areaServices
)
        {
            _user = user;
            _orderServices = orderServices;
            _areaServices = areaServices;

        }




    }
}