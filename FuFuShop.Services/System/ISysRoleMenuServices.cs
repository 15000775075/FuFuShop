
using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     角色菜单关联表 服务工厂接口
    /// </summary>
    public interface ISysRoleMenuServices : IBaseServices<SysRoleMenu>
    {
        Task<List<SysRoleMenu>> RoleModuleMaps();
    }
}