using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     定时任务日志 接口实现
    /// </summary>
    public class SysTaskLogServices : BaseServices<SysTaskLog>, ISysTaskLogServices
    {
        private readonly ISysTaskLogRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysTaskLogServices(IUnitOfWork unitOfWork, ISysTaskLogRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}