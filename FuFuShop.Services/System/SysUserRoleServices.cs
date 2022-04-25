

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     用户角色关联表 接口实现
    /// </summary>
    public class SysUserRoleServices : BaseServices<SysUserRole>, ISysUserRoleServices
    {
        private readonly ISysUserRoleRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysUserRoleServices(IUnitOfWork unitOfWork, ISysUserRoleRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}