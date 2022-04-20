using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 订单表 接口实现
    /// </summary>
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }



        #region 根据用户id和商品id获取下了多少订单
        /// <summary>
        /// 根据用户id和商品id获取下了多少订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodId"></param>
        /// <returns></returns>
        public int GetOrderNum(int userId, int goodId)
        {
            var num = DbClient.Queryable<Order, OrderItem>((op, ot) => new object[]
            {
                JoinType.Inner, op.orderId == ot.orderId
            }).Where((op, ot) => op.userId == userId && ot.goodsId == goodId).Count();
            return num;
        }
        #endregion

        #region 重写根据条件列表数据
        /// <summary>
        ///     重写根据条件列表数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<List<Order>> QueryListAsync(Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType)
        {
            List<Order> list = await DbClient.Queryable<Order, FuFuShopUser>((sOrder, sUser) => new JoinQueryInfos(
                     JoinType.Left, sOrder.userId == sUser.id))
                    .Select((sOrder, sUser) => new Order
                    {
                        orderId = sOrder.orderId,
                        goodsAmount = sOrder.goodsAmount,
                        payedAmount = sOrder.payedAmount,
                        orderAmount = sOrder.orderAmount,
                        payStatus = sOrder.payStatus,
                        shipStatus = sOrder.shipStatus,
                        status = sOrder.status,
                        orderType = sOrder.orderType,
                        receiptType = sOrder.receiptType,
                        paymentCode = sOrder.paymentCode,
                        paymentTime = sOrder.paymentTime,
                        logisticsId = sOrder.logisticsId,
                        logisticsName = sOrder.logisticsName,
                        costFreight = sOrder.costFreight,
                        userId = sOrder.userId,
                        sellerId = sOrder.sellerId,
                        confirmStatus = sOrder.confirmStatus,
                        confirmTime = sOrder.confirmTime,
                        storeId = sOrder.storeId,
                        shipAreaId = sOrder.shipAreaId,
                        shipAddress = sOrder.shipAddress,
                        shipName = sOrder.shipName,
                        shipMobile = sOrder.shipMobile,
                        weight = sOrder.weight,
                        taxType = sOrder.taxType,
                        taxCode = sOrder.taxCode,
                        taxTitle = sOrder.taxTitle,
                        point = sOrder.point,
                        pointMoney = sOrder.pointMoney,
                        orderDiscountAmount = sOrder.orderDiscountAmount,
                        goodsDiscountAmount = sOrder.goodsDiscountAmount,
                        couponDiscountAmount = sOrder.couponDiscountAmount,
                        coupon = sOrder.coupon,
                        promotionList = sOrder.promotionList,
                        memo = sOrder.memo,
                        ip = sOrder.ip,
                        mark = sOrder.mark,
                        source = sOrder.source,
                        isComment = sOrder.isComment,
                        isdel = sOrder.isdel,
                        createTime = sOrder.createTime,
                        updateTime = sOrder.updateTime,
                        userNickName = sUser.nickName
                    })
                    .With(SqlWith.NoLock)
                    .MergeTable()
                    .Mapper(sOrder => sOrder.aftersalesItem, sOrder => sOrder.aftersalesItem.First().orderId)
                    .Mapper(sOrder => sOrder.items, sOrder => sOrder.items.First().orderId)
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToListAsync();
            return list;
        }

        #endregion

        #region 重写根据条件查询分页数据-带用户数据
        /// <summary>
        ///     重写根据条件查询分页数据-带用户数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public new async Task<IPageList<Order>> QueryPageAsync(Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Order> page = await DbClient.Queryable<Order, FuFuShopUser>((sOrder, sUser) => new JoinQueryInfos(
                     JoinType.Left, sOrder.userId == sUser.id))
                    .Select((sOrder, sUser) => new Order
                    {
                        orderId = sOrder.orderId,
                        goodsAmount = sOrder.goodsAmount,
                        payedAmount = sOrder.payedAmount,
                        orderAmount = sOrder.orderAmount,
                        payStatus = sOrder.payStatus,
                        shipStatus = sOrder.shipStatus,
                        status = sOrder.status,
                        orderType = sOrder.orderType,
                        receiptType = sOrder.receiptType,
                        paymentCode = sOrder.paymentCode,
                        paymentTime = sOrder.paymentTime,
                        logisticsId = sOrder.logisticsId,
                        logisticsName = sOrder.logisticsName,
                        costFreight = sOrder.costFreight,
                        userId = sOrder.userId,
                        sellerId = sOrder.sellerId,
                        confirmStatus = sOrder.confirmStatus,
                        confirmTime = sOrder.confirmTime,
                        storeId = sOrder.storeId,
                        shipAreaId = sOrder.shipAreaId,
                        shipAddress = sOrder.shipAddress,
                        shipName = sOrder.shipName,
                        shipMobile = sOrder.shipMobile,
                        weight = sOrder.weight,
                        taxType = sOrder.taxType,
                        taxCode = sOrder.taxCode,
                        taxTitle = sOrder.taxTitle,
                        point = sOrder.point,
                        pointMoney = sOrder.pointMoney,
                        orderDiscountAmount = sOrder.orderDiscountAmount,
                        goodsDiscountAmount = sOrder.goodsDiscountAmount,
                        couponDiscountAmount = sOrder.couponDiscountAmount,
                        coupon = sOrder.coupon,
                        promotionList = sOrder.promotionList,
                        memo = sOrder.memo,
                        ip = sOrder.ip,
                        mark = sOrder.mark,
                        source = sOrder.source,
                        isComment = sOrder.isComment,
                        isdel = sOrder.isdel,
                        createTime = sOrder.createTime,
                        updateTime = sOrder.updateTime,
                        userNickName = sUser.nickName
                    })
                    .MergeTable()
                    .Mapper(sOrder => sOrder.aftersalesItem, sOrder => sOrder.aftersalesItem.First().orderId)
                    .Mapper(sOrder => sOrder.items, sOrder => sOrder.items.First().orderId)
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);

            var list = new PageList<Order>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


        #region 重写根据条件查询分页数据
        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<IPageList<Order>> QueryPageNewAsync(Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<Order> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Order>()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate).Select(p => new Order
                {
                    orderId = p.orderId,
                    goodsAmount = p.goodsAmount,
                    payedAmount = p.payedAmount,
                    orderAmount = p.orderAmount,
                    payStatus = p.payStatus,
                    shipStatus = p.shipStatus,
                    status = p.status,
                    orderType = p.orderType,
                    receiptType = p.receiptType,
                    paymentCode = p.paymentCode,
                    paymentTime = p.paymentTime,
                    logisticsId = p.logisticsId,
                    logisticsName = p.logisticsName,
                    costFreight = p.costFreight,
                    userId = p.userId,
                    sellerId = p.sellerId,
                    confirmStatus = p.confirmStatus,
                    confirmTime = p.confirmTime,
                    storeId = p.storeId,
                    shipAreaId = p.shipAreaId,
                    shipAddress = p.shipAddress,
                    shipName = p.shipName,
                    shipMobile = p.shipMobile,
                    weight = p.weight,
                    taxType = p.taxType,
                    taxCode = p.taxCode,
                    taxTitle = p.taxTitle,
                    point = p.point,
                    pointMoney = p.pointMoney,
                    orderDiscountAmount = p.orderDiscountAmount,
                    goodsDiscountAmount = p.goodsDiscountAmount,
                    couponDiscountAmount = p.couponDiscountAmount,
                    coupon = p.coupon,
                    promotionList = p.promotionList,
                    memo = p.memo,
                    ip = p.ip,
                    mark = p.mark,
                    source = p.source,
                    isComment = p.isComment,
                    isdel = p.isdel,
                    createTime = p.createTime,
                    updateTime = p.updateTime,

                }).With(SqlWith.NoLock).ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Order>()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate).Select(p => new Order
                {
                    orderId = p.orderId,
                    goodsAmount = p.goodsAmount,
                    payedAmount = p.payedAmount,
                    orderAmount = p.orderAmount,
                    payStatus = p.payStatus,
                    shipStatus = p.shipStatus,
                    status = p.status,
                    orderType = p.orderType,
                    receiptType = p.receiptType,
                    paymentCode = p.paymentCode,
                    paymentTime = p.paymentTime,
                    logisticsId = p.logisticsId,
                    logisticsName = p.logisticsName,
                    costFreight = p.costFreight,
                    userId = p.userId,
                    sellerId = p.sellerId,
                    confirmStatus = p.confirmStatus,
                    confirmTime = p.confirmTime,
                    storeId = p.storeId,
                    shipAreaId = p.shipAreaId,
                    shipAddress = p.shipAddress,
                    shipName = p.shipName,
                    shipMobile = p.shipMobile,
                    weight = p.weight,
                    taxType = p.taxType,
                    taxCode = p.taxCode,
                    taxTitle = p.taxTitle,
                    point = p.point,
                    pointMoney = p.pointMoney,
                    orderDiscountAmount = p.orderDiscountAmount,
                    goodsDiscountAmount = p.goodsDiscountAmount,
                    couponDiscountAmount = p.couponDiscountAmount,
                    coupon = p.coupon,
                    promotionList = p.promotionList,
                    memo = p.memo,
                    ip = p.ip,
                    mark = p.mark,
                    source = p.source,
                    isComment = p.isComment,
                    isdel = p.isdel,
                    createTime = p.createTime,
                    updateTime = p.updateTime,

                }).ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<Order>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


    }
}

