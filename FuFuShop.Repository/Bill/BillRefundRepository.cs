
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 退款单表 接口实现
    /// </summary>
    public class BillRefundRepository : BaseRepository<BillRefund>, IBillRefundRepository
    {
        public BillRefundRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

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
        public new async Task<IPageList<BillRefund>> QueryPageAsync(Expression<Func<BillRefund, bool>> predicate,
            Expression<Func<BillRefund, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<BillRefund> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<BillRefund, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                        JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new BillRefund
                    {
                        refundId = p.refundId,
                        aftersalesId = p.aftersalesId,
                        money = p.money,
                        userId = p.userId,
                        sourceId = p.sourceId,
                        type = p.type,
                        paymentCode = p.paymentCode,
                        tradeNo = p.tradeNo,
                        status = p.status,
                        memo = p.memo,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .With(SqlWith.NoLock).ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<BillRefund, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                        JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new BillRefund
                    {
                        refundId = p.refundId,
                        aftersalesId = p.aftersalesId,
                        money = p.money,
                        userId = p.userId,
                        sourceId = p.sourceId,
                        type = p.type,
                        paymentCode = p.paymentCode,
                        tradeNo = p.tradeNo,
                        status = p.status,
                        memo = p.memo ?? "",
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<BillRefund>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


    }
}
