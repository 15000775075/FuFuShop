

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     角色表 接口实现
    /// </summary>
    public class SysRoleServices : BaseServices<SysRole>, ISysRoleServices
    {
        private readonly ISysRoleRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysRoleServices(IUnitOfWork unitOfWork, ISysRoleRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}