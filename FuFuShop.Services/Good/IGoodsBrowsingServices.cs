
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     商品浏览记录表 服务工厂接口
    /// </summary>
    public interface IGoodsBrowsingServices : IBaseServices<GoodsBrowsing>
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
        Task<IPageList<GoodsBrowsing>> QueryPageAsync(Expression<Func<GoodsBrowsing, bool>> predicate,
            Expression<Func<GoodsBrowsing, object>> orderByExpression, OrderByType orderByType,
            int pageIndex = 1,
            int pageSize = 20);
    }
}