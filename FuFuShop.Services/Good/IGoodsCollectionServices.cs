using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;
using GoodsCollection = FuFuShop.Model.Entities.GoodsCollection;

namespace FuFuShop.Services.Good
{
    /// <summary>
    ///     商品收藏表 服务工厂接口
    /// </summary>
    public interface IGoodsCollectionServices : IBaseServices<GoodsCollection>
    {
        /// <summary>
        ///     检查是否收藏了此商品
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        Task<bool> Check(int userId, int goodsId);


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