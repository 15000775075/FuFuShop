using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     微信支付 工厂接口
    /// </summary>
    public interface IWeChatPayRepository : IBaseRepository<Setting>
    {
    }
}