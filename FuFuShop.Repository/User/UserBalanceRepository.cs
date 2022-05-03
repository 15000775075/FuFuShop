/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     用户余额表 接口实现
    /// </summary>
    public class UserBalanceRepository : BaseRepository<UserBalance>, IUserBalanceRepository
    {
        public UserBalanceRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public new async Task<IPageList<UserBalance>> QueryPageAsync(
            Expression<Func<UserBalance, bool>> predicate,
            Expression<Func<UserBalance, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<UserBalance> page;
            if (blUseNoLock)
                page = await DbClient.Queryable<UserBalance, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                        JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new UserBalance
                    {
                        id = p.id,
                        userId = p.userId,
                        type = p.type,
                        money = p.money,
                        balance = p.balance,
                        sourceId = p.sourceId,
                        memo = p.memo,
                        createTime = p.createTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .With(SqlWith.NoLock).ToPageListAsync(pageIndex, pageSize, totalCount);
            else
                page = await DbClient.Queryable<UserBalance, FuFuShopUser>((p, sc) => new JoinQueryInfos(
                        JoinType.Left, p.userId == sc.id))
                    .Select((p, sc) => new UserBalance
                    {
                        id = p.id,
                        userId = p.userId,
                        type = p.type,
                        money = p.money,
                        balance = p.balance,
                        sourceId = p.sourceId,
                        memo = p.memo,
                        createTime = p.createTime,
                        userNickName = sc.nickName
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            var list = new PageList<UserBalance>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion
    }
}