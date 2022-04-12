
using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     门店表 服务工厂接口
    /// </summary>
    public interface IStoreServices : IBaseServices<Store>
    {
        /// <summary>
        ///     事务重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> InsertAsync(Store entity);


        /// <summary>
        ///     重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(Store entity);


        /// <summary>
        ///     Sql根据条件查询分页数据带距离
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">精度</param>
        /// <returns></returns>
        Task<IPageList<Store>> QueryPageAsyncByCoordinate(
            Expression<Func<Store, bool>> predicate,
            Expression<Func<Store, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, decimal latitude = 0, decimal longitude = 0);


        /// <summary>
        ///     根据用户序列获取单个门店数据
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        Task<Store> GetStoreByUserId(int userId, bool blUseNoLock = false);
    }
}