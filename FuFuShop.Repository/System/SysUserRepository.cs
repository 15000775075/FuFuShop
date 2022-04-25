
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     用户表 接口实现
    /// </summary>
    public class SysUserRepository : BaseRepository<SysUser>, ISysUserRepository
    {
        public SysUserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}