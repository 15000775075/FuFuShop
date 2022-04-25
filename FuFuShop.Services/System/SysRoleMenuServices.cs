/***********************************************************************
 *            Project: 
 *        ProjectName: 核心内容管理系统                                
 *                Web: https://www..net                      
 *             Author: 大灰灰                                          
 *              Email: jianweie@163.com                                
 *         CreateTime: 2021/1/31 21:45:10
 *        Description: 暂无 ***********************************************************************/

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     角色菜单关联表 接口实现
    /// </summary>
    public class SysRoleMenuServices : BaseServices<SysRoleMenu>, ISysRoleMenuServices
    {
        private readonly ISysRoleMenuRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysRoleMenuServices(IUnitOfWork unitOfWork, ISysRoleMenuRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     角色权限Map
        ///     RoleModulePermission, Module, Role 三表联合
        ///     第四个类型 RoleModulePermission 是返回值
        /// </summary>
        /// <returns></returns>
        public async Task<List<SysRoleMenu>> RoleModuleMaps()
        {
            return await _dal.RoleModuleMaps();
        }
    }
}