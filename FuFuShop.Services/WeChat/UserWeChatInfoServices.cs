using FuFuShop.Model.Entities;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Repository.WeChat;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.WeChat
{
    /// <summary>
    /// 用户表 接口实现
    /// </summary>
    public class UserWeChatInfoServices : BaseServices<UserWeChatInfo>, IUserWeChatInfoServices
    {
        private readonly IUserWeChatInfoRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public UserWeChatInfoServices(IUnitOfWork unitOfWork, IUserWeChatInfoRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

    }
}
