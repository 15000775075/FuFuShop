/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

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
    /// 库存操作详情表 接口实现
    /// </summary>
    public class StockLogRepository : BaseRepository<StockLog>, IStockLogRepository
    {
        public StockLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region 实现重写增删改查操作==========================================================

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> InsertAsync(StockLog entity)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Insertable(entity).ExecuteReturnIdentityAsync() > 0;
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.CreateSuccess : GlobalConstVars.CreateFailure;
            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(StockLog entity)
        {
            var jm = new AdminUiCallBack();

            var oldModel = await DbClient.Queryable<StockLog>().In(entity.id).SingleAsync();
            if (oldModel == null)
            {
                jm.msg = "不存在此信息";
                return jm;
            }
            //事物处理过程开始
            oldModel.id = entity.id;
            oldModel.stockId = entity.stockId;
            oldModel.productId = entity.productId;
            oldModel.goodsId = entity.goodsId;
            oldModel.nums = entity.nums;
            oldModel.sn = entity.sn;
            oldModel.bn = entity.bn;
            oldModel.goodsName = entity.goodsName;
            oldModel.spesDesc = entity.spesDesc;

            //事物处理过程结束
            var bl = await DbClient.Updateable(oldModel).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(List<StockLog> entity)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Updateable(entity).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.EditSuccess : GlobalConstVars.EditFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdAsync(object id)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<StockLog>(id).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
        }

        /// <summary>
        /// 重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids)
        {
            var jm = new AdminUiCallBack();

            var bl = await DbClient.Deleteable<StockLog>().In(ids).ExecuteCommandHasChangeAsync();
            jm.code = bl ? 0 : 1;
            jm.msg = bl ? GlobalConstVars.DeleteSuccess : GlobalConstVars.DeleteFailure;

            return jm;
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
        public new async Task<IPageList<StockLog>> QueryPageAsync(Expression<Func<StockLog, bool>> predicate,
            Expression<Func<StockLog, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false)
        {
            RefAsync<int> totalCount = 0;
            List<StockLog> page;
            if (blUseNoLock)
            {
                page = await DbClient.Queryable<StockLog, Stock>((p, stock) => new JoinQueryInfos(JoinType.Left, p.stockId == stock.id))
                .Select((p, stock) => new StockLog
                {
                    id = p.id,
                    stockId = p.stockId,
                    productId = p.productId,
                    goodsId = p.goodsId,
                    nums = p.nums,
                    sn = p.sn,
                    bn = p.bn,
                    goodsName = p.goodsName,
                    spesDesc = p.spesDesc,
                    type = stock.type,
                    manager = stock.manager,
                    memo = stock.memo,
                    createTime = stock.createTime

                }).With(SqlWith.NoLock)
                .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate)
                .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            else
            {
                page = await DbClient.Queryable<StockLog, Stock>((p, stock) => new JoinQueryInfos(JoinType.Left, p.stockId == stock.id))
                    .Select((p, stock) => new StockLog
                    {
                        id = p.id,
                        stockId = p.stockId,
                        productId = p.productId,
                        goodsId = p.goodsId,
                        nums = p.nums,
                        sn = p.sn,
                        bn = p.bn,
                        goodsName = p.goodsName,
                        spesDesc = p.spesDesc,
                        type = stock.type,
                        manager = stock.manager,
                        memo = stock.memo,
                        createTime = stock.createTime
                    })
                    .MergeTable()
                    .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                    .WhereIF(predicate != null, predicate)
                    .ToPageListAsync(pageIndex, pageSize, totalCount);
            }
            var list = new PageList<StockLog>(page, pageIndex, pageSize, totalCount);
            return list;
        }

        #endregion

    }
}
