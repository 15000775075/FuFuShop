

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
{
    /// <summary>
    /// 登录日志表 接口实现
    /// </summary>
    public class SysLoginRecordServices : BaseServices<SysLoginRecord>, ISysLoginRecordServices
    {
        private readonly ISysLoginRecordRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public SysLoginRecordServices(IUnitOfWork unitOfWork, ISysLoginRecordRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }



    }
}
