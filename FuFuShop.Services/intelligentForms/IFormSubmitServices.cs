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
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

namespace FuFuShop.Services
{
    /// <summary>
    ///     用户对表的提交记录 服务工厂接口
    /// </summary>
    public interface IFormSubmitServices : IBaseServices<FormSubmit>
    {
        #region 重写根据条件查询分页数据

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
        new Task<IPageList<FormSubmit>> QueryPageAsync(
            Expression<Func<FormSubmit, bool>> predicate,
            Expression<Func<FormSubmit, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);

        #endregion


        /// <summary>
        ///     表单支付
        /// </summary>
        /// <param name="id">序列</param>
        /// <returns></returns>
        Task<WebApiCallBack> Pay(int id);


        /// <summary>
        ///     获取表单的统计数据
        /// </summary>
        /// <param name="formId">表单序列</param>
        /// <param name="day">多少天内的数据</param>
        /// <returns></returns>
        Task<FormStatisticsViewDto> GetStatisticsByFormid(int formId, int day);

        #region 重写增删改查操作===========================================================

        /// <summary>
        ///     重写异步插入方法并返回自增值
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task<int> InsertReturnIdentityAsync(FormSubmit entity);


        /// <summary>
        ///     重写异步插入方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> InsertAsync(FormSubmit entity);

        /// <summary>
        ///     重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(FormSubmit entity);

        /// <summary>
        ///     重写异步更新方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> UpdateAsync(List<FormSubmit> entity);

        /// <summary>
        ///     重写删除指定ID的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> DeleteByIdAsync(object id);

        /// <summary>
        ///     重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        new Task<AdminUiCallBack> DeleteByIdsAsync(int[] ids);

        #endregion
    }
}