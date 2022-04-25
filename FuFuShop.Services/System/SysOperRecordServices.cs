

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     操作日志表 接口实现
    /// </summary>
    public class SysOperRecordServices : BaseServices<SysOperRecord>, ISysOperRecordServices
    {
        private readonly ISysOperRecordRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public SysOperRecordServices(IUnitOfWork unitOfWork, ISysOperRecordRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}