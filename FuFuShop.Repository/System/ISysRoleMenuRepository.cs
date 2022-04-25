

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     角色菜单关联表 工厂接口
    /// </summary>
    public interface ISysRoleMenuRepository : IBaseRepository<SysRoleMenu>
    {
        Task<List<SysRoleMenu>> RoleModuleMaps();
    }
}