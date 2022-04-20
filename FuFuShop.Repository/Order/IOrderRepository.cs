using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Repository.BaseRepository;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     订单表 工厂接口
    /// </summary>
    public interface IOrderRepository : IBaseRepository<Order>
    {


        /// <summary>
        ///     根据用户id和商品id获取下了多少订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodId"></param>
        /// <returns></returns>
        int GetOrderNum(int userId, int goodId);

        /// <summary>
        ///     重写根据条件列表数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<List<Order>> QueryListAsync(Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType);

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
        new Task<IPageList<Order>> QueryPageAsync(
            Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);


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
        Task<IPageList<Order>> QueryPageNewAsync(Expression<Func<Order, bool>> predicate,
            Expression<Func<Order, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);
    }


}