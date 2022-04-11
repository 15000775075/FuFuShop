/***********************************************************************
 *            Project: CoreCms
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www.corecms.net                      
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
    ///     商品浏览记录表 接口实现
    /// </summary>
    public class GoodsBrowsingRepository : BaseRepository<GoodsBrowsing>, IGoodsBrowsingRepository
    {
        public GoodsBrowsingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        public async Task<IPageList<GoodsBrowsing>> QueryPageAsync(
            Expression<Func<GoodsBrowsing, bool>> predicate,
            Expression<Func<GoodsBrowsing, object>> orderByExpression, OrderByType orderByType,
            int pageIndex = 1,
            int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            var page = await DbClient.Queryable<GoodsBrowsing, Goods>((gb, goods) =>
                    new JoinQueryInfos(JoinType.Left, gb.goodsId == goods.id))
                .Select((gb, goods) => new GoodsBrowsing
                {
                    id = gb.id,
                    goodsId = gb.goodsId,
                    userId = gb.userId,
                    goodsName = gb.goodsName,
                    createTime = gb.createTime,
                    isdel = gb.isdel,
                    goodImage = goods.image
                    //isCollection = SqlFunc.Subqueryable<CoreCmsGoodsCollection>().Where(p => p.userId == gb.userId && p.goodsId == gb.goodsId).Any()
                })
                .With(SqlWith.NoLock)
                .MergeTable()
                .OrderByIF(orderByExpression != null, orderByExpression, orderByType)
                .WhereIF(predicate != null, predicate).ToPageListAsync(pageIndex, pageSize, totalCount);

            var list = new PageList<GoodsBrowsing>(page, pageIndex, pageSize, totalCount);
            return list;
        }
    }
}