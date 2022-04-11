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
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 店铺店员关联表 接口实现
    /// </summary>
    public class ClerkRepository : BaseRepository<Clerk>, IClerkRepository
    {
        public ClerkRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }



        #region 获取门店关联用户分页数据
        /// <summary>
        ///     获取门店关联用户分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<IPageList<StoreClerkDto>> QueryStoreClerkDtoPageAsync(Expression<Func<StoreClerkDto, bool>> predicate,
            Expression<Func<StoreClerkDto, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<StoreClerkDto> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<Clerk, Store, FuFuShopUser>((p, sst, ccu) => new JoinQueryInfos(
                        JoinType.Left, p.storeId == sst.id,
                        JoinType.Left, p.userId == ccu.id
                        ))
                    .Select((p, sst, ccu) => new StoreClerkDto
                    {
                        id = p.id,
                        storeId = p.storeId,
                        userId = p.userId,
                        isDel = p.isDel,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        storeName = sst.storeName,
                        nickName = ccu.nickName,
                        mobile = ccu.mobile,
                        avatarImage = ccu.avatarImage,
                    }).With(SqlWith.NoLock)
                .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<Clerk, Store, FuFuShopUser>((p, sst, ccu) => new JoinQueryInfos(
                        JoinType.Left, p.storeId == sst.id,
                        JoinType.Left, p.userId == ccu.id
                    ))
                    .Select((p, sst, ccu) => new StoreClerkDto
                    {
                        id = p.id,
                        storeId = p.storeId,
                        userId = p.userId,
                        isDel = p.isDel,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        storeName = sst.storeName,
                        nickName = ccu.nickName,
                        mobile = ccu.mobile,
                        avatarImage = ccu.avatarImage,
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<StoreClerkDto>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion


        #region 获取单个门店用户数据
        /// <summary>
        ///     获取单个门店用户数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<StoreClerkDto> QueryStoreClerkDtoByClauseAsync(Expression<Func<StoreClerkDto, bool>> predicate, bool blUseNoLock = false)
        {
            StoreClerkDto obj;
            if (blUseNoLock)
            {
                obj = await DbClient.Queryable<Clerk, Store, FuFuShopUser>((p, sst, ccu) => new JoinQueryInfos(
                        JoinType.Left, p.storeId == sst.id,
                        JoinType.Left, p.userId == ccu.id
                        ))
                    .Select((p, sst, ccu) => new StoreClerkDto
                    {
                        id = p.id,
                        storeId = p.storeId,
                        userId = p.userId,
                        isDel = p.isDel,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        storeName = sst.storeName,
                        nickName = ccu.nickName,
                        mobile = ccu.mobile,
                        avatarImage = ccu.avatarImage,
                    }).With(SqlWith.NoLock)
                .MergeTable()
                .FirstAsync(predicate);
            }
            else
            {
                obj = await DbClient.Queryable<Clerk, Store, FuFuShopUser>((p, sst, ccu) => new JoinQueryInfos(
                        JoinType.Left, p.storeId == sst.id,
                        JoinType.Left, p.userId == ccu.id
                    ))
                    .Select((p, sst, ccu) => new StoreClerkDto
                    {
                        id = p.id,
                        storeId = p.storeId,
                        userId = p.userId,
                        isDel = p.isDel,
                        createTime = p.createTime,
                        updateTime = p.updateTime,
                        storeName = sst.storeName,
                        nickName = ccu.nickName,
                        mobile = ccu.mobile,
                        avatarImage = ccu.avatarImage,
                    })
                    .MergeTable()
                    .FirstAsync(predicate);

            }
            return obj;
        }
        #endregion


    }
}
