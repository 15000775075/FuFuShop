

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     微信授权交互 接口实现
    /// </summary>
    public class WeChatAccessTokenServices : BaseServices<WeChatAccessToken>, IWeChatAccessTokenServices
    {
        private readonly IWeChatAccessTokenRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public WeChatAccessTokenServices(IUnitOfWork unitOfWork, IWeChatAccessTokenRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}