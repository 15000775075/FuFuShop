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
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 用户地址表 接口实现
    /// </summary>
    public class UserShipServices : BaseServices<UserShip>, IUserShipServices
    {
        private readonly IUserShipRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public UserShipServices(IUnitOfWork unitOfWork, IUserShipRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 重写异步插入方法
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public new async Task<WebApiCallBack> InsertAsync(UserShip entity)
        {
            return await _dal.InsertAsync(entity);
        }



        /// <summary>
        /// 重写异步更新方法方法
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public new async Task<AdminUiCallBack> UpdateAsync(UserShip entity)
        {
            return await _dal.UpdateAsync(entity);
        }
    }
}
