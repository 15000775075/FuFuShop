/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无
 ***********************************************************************/

using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     用户表 工厂接口
    /// </summary>
    public interface IUserRepository : IBaseRepository<FuFuShopUser>
    {
        /// <summary>
        ///     获取下级推广用户数量
        /// </summary>
        /// <param name="parentId">父类序列</param>
        /// <param name="type">1获取1级，其他为2级</param>
        /// <param name="thisMonth">当月</param>
        /// <returns></returns>
        Task<int> QueryChildCountAsync(int parentId, int type = 1, bool thisMonth = false);


        /// <summary>
        ///     根据条件查询分页数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <returns></returns>
        Task<IPageList<FuFuShopUser>> QueryPageAsync(Expression<Func<FuFuShopUser, bool>> predicate,
            Expression<Func<FuFuShopUser, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20);

        /// <summary>
        ///     按天统计新会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> Statistics(int day);


        /// <summary>
        ///     按天统计当天下单活跃会员
        /// </summary>
        /// <returns></returns>
        Task<List<StatisticsOut>> StatisticsOrder(int day);
    }
}