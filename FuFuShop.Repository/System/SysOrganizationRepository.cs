

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     组织机构表 接口实现
    /// </summary>
    public class SysOrganizationRepository : BaseRepository<SysOrganization>, ISysOrganizationRepository
    {
        public SysOrganizationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}