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
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     售后单明细表 接口实现
    /// </summary>
    public class BillAftersalesItemServices : BaseServices<BillAftersalesItem>,
        IBillAftersalesItemServices
    {
        private readonly IBillAftersalesItemRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public BillAftersalesItemServices(IUnitOfWork unitOfWork, IBillAftersalesItemRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}