using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     订单明细表 工厂接口
    /// </summary>
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {

        /// <summary>
        ///     算订单的商品退了多少个(未发货的退货数量，已发货的退货不算)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        int GetaftersalesNums(string orderId, string sn);
    }
}