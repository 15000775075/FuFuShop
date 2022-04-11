using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 退货单表 接口实现
    /// </summary>
    public class BillReshipRepository : BaseRepository<BillReship>, IBillReshipRepository
    {
        public BillReshipRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// 获取单个数据带导航
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="orderByType"></param>
        /// <returns></returns>
        public async Task<BillReship> GetDetails(Expression<Func<BillReship, bool>> predicate,
            Expression<Func<BillReship, object>> orderByExpression, OrderByType orderByType)
        {
            var model = await DbClient.Queryable<BillReship, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                    JoinType.Left, p.userId == sc.id))
                .Select((p, sc) => new BillReship
                {
                    reshipId = p.reshipId,
                    orderId = p.orderId,
                    aftersalesId = p.aftersalesId,
                    userId = p.userId,
                    logiCode = p.logiCode,
                    logiNo = p.logiNo,
                    status = p.status,
                    memo = p.memo,
                    createTime = p.createTime,
                    updateTime = p.updateTime,
                    userNickName = sc.nickName
                })
                .MergeTable()
                .Mapper(p => p.items, p => p.reshipId)
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .FirstAsync();

            return model;
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
        /// <returns></returns>
        public async Task<IPageList<BillReship>> QueryPageAsync(Expression<Func<BillReship, bool>> predicate,
            Expression<Func<BillReship, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            List<BillReship> page = await DbClient.Queryable<BillReship, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                     JoinType.Left, p.userId == sc.id))
               .Select((p, sc) => new BillReship
               {
                   reshipId = p.reshipId,
                   orderId = p.orderId,
                   aftersalesId = p.aftersalesId,
                   userId = p.userId,
                   logiCode = p.logiCode,
                   logiNo = p.logiNo,
                   status = p.status,
                   memo = p.memo,
                   createTime = p.createTime,
                   updateTime = p.updateTime,
                   userNickName = sc.nickName
               })
               .MergeTable()
               .Mapper(p => p.items, p => p.reshipId)
               .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
               .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);

            var list = new PageList<BillReship>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion

    }
}
