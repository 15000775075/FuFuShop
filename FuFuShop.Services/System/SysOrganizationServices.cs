

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     组织机构表 接口实现
    /// </summary>
    public class SysOrganizationServices : BaseServices<SysOrganization>, ISysOrganizationServices
    {
        private readonly ISysOrganizationRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysOrganizationServices(IUnitOfWork unitOfWork, ISysOrganizationRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}