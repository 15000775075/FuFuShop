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
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    /// 门店表 接口实现
    /// </summary>
    public class StoreServices : BaseServices<Store>, IStoreServices
    {
        private readonly IStoreRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public StoreServices(IUnitOfWork unitOfWork, IStoreRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> InsertAsync(Store entity)
        {
            return await _dal.InsertAsync(entity);
        }

        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(Store entity)
        {
            return await _dal.UpdateAsync(entity);
        }

        #region Sql根据条件查询分页数据带距离

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
        public async Task<IPageList<Store>> QueryPageAsyncByCoordinate(Expression<Func<Store, bool>> predicate,
            Expression<Func<Store, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, decimal latitude = 0, decimal longitude = 0)
        {
            return await _dal.QueryPageAsyncByCoordinate(predicate, orderByExpression, orderByType, pageIndex, pageSize, latitude, longitude);
        }
        #endregion

        /// <summary>
        ///     根据用户序列获取单个门店数据
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        public async Task<Store> GetStoreByUserId(int userId, bool blUseNoLock = false)
        {
            return await _dal.GetStoreByUserId(userId, blUseNoLock);
        }

    }
}
