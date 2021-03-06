using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using SqlSugar;
using System.Linq.Expressions;
using GoodsCollection = FuFuShop.Model.Entities.GoodsCollection;

namespace FuFuShop.Repository.Good
{
    /// <summary>
    ///     商品收藏表 工厂接口
    /// </summary>
    public interface IGoodsCollectionRepository : IBaseRepository<GoodsCollection>
    {
        /// <summary>
        ///     重写根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<IPageList<GoodsCollection>> QueryPageAsync(
            Expression<Func<GoodsCollection, bool>> predicate,
            Expression<Func<GoodsCollection, object>> orderByExpression, OrderByType orderByType,
            int pageIndex = 1,
            int pageSize = 20);


        /// <summary>
        ///     收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> ToAdd(int userId, int goodsId);
    }
}