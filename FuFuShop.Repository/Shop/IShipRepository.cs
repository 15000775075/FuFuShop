using FuFuShop.Model.Entities.Shop;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository.Shop
{
    /// <summary>
    ///     配送方式表 工厂接口
    /// </summary>
    public interface IShipRepository : IBaseRepository<Ship>
    {
        /// <summary>
        ///     设置是否默认
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> SetIsDefault(int id, bool isDefault);

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
        new Task<IPageList<Ship>> QueryPageAsync(
            Expression<Func<Ship, bool>> predicate,
            Expression<Func<Ship, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);

        #region 重写增删改查操作===========================================================

        /// <summary>
        ///     重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> InsertAsync(Ship entity);


        /// <summary>
        ///     重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(Ship entity);

        /// <summary>
        ///     重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> DeleteByIdAsync(int id);

        #endregion
    }
}