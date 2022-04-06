using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     微信授权交互 接口实现
    /// </summary>
    public class WeChatAccessTokenRepository : BaseRepository<WeChatAccessToken>, IWeChatAccessTokenRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public WeChatAccessTokenRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}