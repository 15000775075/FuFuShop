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
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    /// 商品浏览记录表 接口实现
    /// </summary>
    public class GoodsBrowsingServices : BaseServices<GoodsBrowsing>, IGoodsBrowsingServices
    {
        private readonly IGoodsBrowsingRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public GoodsBrowsingServices(IUnitOfWork unitOfWork, IGoodsBrowsingRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
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
        public async Task<IPageList<GoodsBrowsing>> QueryPageAsync(Expression<Func<GoodsBrowsing, bool>> predicate,
            Expression<Func<GoodsBrowsing, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20)
        {
            return await _dal.QueryPageAsync(predicate, orderByExpression, orderByType, pageIndex, pageSize);
        }

    }
}
