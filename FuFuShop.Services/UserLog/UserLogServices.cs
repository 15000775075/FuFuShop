using CoreCms.Net.IRepository;
using CoreCms.Net.IServices;
using FuFuShop.Model.Entitys;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace CoreCms.Net.Services
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
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
