

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     用户角色关联表 接口实现
    /// </summary>
    public class SysUserRoleRepository : BaseRepository<SysUserRole>, ISysUserRoleRepository
    {
        public SysUserRoleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}