using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 支付单表 接口实现
    /// </summary>
    public class BillPaymentsRepository : BaseRepository<BillPayments>, IBillPaymentsRepository
    {

        public BillPaymentsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }


        /// <summary>
        /// 支付单7天统计
        /// </summary>
        /// <returns></returns>
        public async Task<List<StatisticsOut>> Statistics()
        {
            var dt = DateTime.Now.AddDays(-8);

            var list = await DbClient.Queryable<BillPayments>()
                .Where(p => p.createTime >= dt && p.status == (int)GlobalEnumVars.BillPaymentsStatus.Payed && p.type == (int)GlobalEnumVars.BillPaymentsType.Order)
                .Select(it => new
                {
                    it.paymentId,
                    createTime = it.createTime.Date
                })
                .MergeTable()
                .GroupBy(it => it.createTime)
                .Select(it => new StatisticsOut { day = it.createTime.ToString("yyyy-MM-dd"), nums = SqlFunc.AggregateCount(it.paymentId) })
                .ToListAsync();

            return list;
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
        public new async Task<IPageList<BillPayments>> QueryPageAsync(Expression<Func<BillPayments, bool>> predicate,
            Expression<Func<BillPayments, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<BillPayments> page;
            if (blUseNoLock)
                page = await DbClient.Queryable<BillPayments, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                         JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new BillPayments
                    {
                        paymentId = p.paymentId,
                        money = p.money,
                        userId = p.userId,
                        type = p.type,
                        status = p.status,
                        paymentCode = p.paymentCode,
                        ip = p.ip,
                        parameters = p.parameters,
                        payedMsg = p.payedMsg,
                        tradeNo = p.tradeNo,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .With(SqlWith.NoLock).ToPageListAsync(pageIndex, pageSize, totalCount);
            else
                page = await DbClient.Queryable<BillPayments, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                        JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new BillPayments
                    {
                        paymentId = p.paymentId,
                        money = p.money,
                        userId = p.userId,
                        type = p.type,
                        status = p.status,
                        paymentCode = p.paymentCode,
                        ip = p.ip,
                        parameters = p.parameters,
                        payedMsg = p.payedMsg,
                        tradeNo = p.tradeNo,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            var list = new PageList<BillPayments>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


    }
}
