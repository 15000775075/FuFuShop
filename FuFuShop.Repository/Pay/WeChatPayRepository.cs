
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 微信支付 接口实现
    /// </summary>
    public class WeChatPayRepository : BaseRepository<Setting>, IWeChatPayRepository
    {
        public WeChatPayRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}