
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     店铺店员关联表 服务工厂接口
    /// </summary>
    public interface IClerkServices : IBaseServices<Clerk>
    {
        /// <summary>
        ///     判断是不是店员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> IsClerk(int userId);

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
        Task<IPageList<StoreClerkDto>> QueryStoreClerkDtoPageAsync(Expression<Func<StoreClerkDto, bool>> predicate,
            Expression<Func<StoreClerkDto, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);


        /// <summary>
        ///     获取单个门店用户数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        Task<StoreClerkDto> QueryStoreClerkDtoByClauseAsync(Expression<Func<StoreClerkDto, bool>> predicate,
            bool blUseNoLock = false);
    }
}