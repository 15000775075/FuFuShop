using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 订单明细表 接口实现
    /// </summary>
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        #region 算订单的商品退了多少个(未发货的退货数量，已发货的退货不算)

        /// <summary>
        /// 算订单的商品退了多少个(未发货的退货数量，已发货的退货不算)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public int GetaftersalesNums(string orderId, string sn)
        {
            var sum = DbClient.Queryable<BillAftersalesItem, BillAftersales>((item, parent) =>
                    new object[]
                    {
                        JoinType.Inner, item.aftersalesId == parent.aftersalesId
                    }).Where((item, parent) => parent.orderId == orderId)
                .Where((item, parent) => parent.status == (int)GlobalEnumVars.OrderStatus.Complete)
                .Where((item, parent) => item.sn == sn)
                .Where((item, parent) => parent.type == (int)GlobalEnumVars.BillAftersalesStatus.WaitAudit)
                .Sum((item, parent) => item.nums);
            return sum;
        }
        #endregion
    }
}
