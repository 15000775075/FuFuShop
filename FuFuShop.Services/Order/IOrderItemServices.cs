using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     订单明细表 服务工厂接口
    /// </summary>
    public interface IOrderItemServices : IBaseServices<OrderItem>
    {
        /// <summary>
        ///     发货数量
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="item">发货明细</param>
        /// <returns></returns>
        Task<bool> ship(string orderId, Dictionary<int, int> item);
    }
}