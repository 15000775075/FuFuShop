
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Services.BaseServices;
using Microsoft.AspNetCore.Http;


namespace FuFuShop.Services
{
    /// <summary>
    /// 订单表 接口实现
    /// </summary>
    public class OrderServices : BaseServices<Order>, IOrderServices
    {
        private readonly IOrderRepository _dal;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IOrderItemServices _orderItemServices;
        private readonly ILogServices _orderLogServices;


        public OrderServices(IOrderRepository dal
            , IHttpContextAccessor httpContextAccessor
            , IOrderItemServices orderItemServices
            , ILogServices orderLogServices)
        {
            _dal = dal;
            base.BaseDal = dal;

            _httpContextAccessor = httpContextAccessor;

            _orderItemServices = orderItemServices;
            _orderLogServices = orderLogServices;

        }

    }
}
