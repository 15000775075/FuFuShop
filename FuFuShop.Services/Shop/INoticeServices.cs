using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     公告表 服务工厂接口
    /// </summary>
    public interface INoticeServices : IBaseServices<Notice>
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
        Task<IPageList<Notice>> QueryPageAsync(Expression<Func<Notice, bool>> predicate,
            Expression<Func<Notice, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20);


        /// <summary>
        ///     获取列表首页用
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<List<Notice>> QueryListAsync(Expression<Func<Notice, bool>> predicate,
            Expression<Func<Notice, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20);
    }
}