using FuFuShop.Model.Entitys;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 用户日志 接口实现
    /// </summary>
    public class UserLogServices : BaseServices<UserLog>, IUserLogServices
    {
        private readonly IUserLogRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public UserLogServices(IUnitOfWork unitOfWork, IUserLogRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
