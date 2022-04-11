using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     支付宝支付 工厂接口
    /// </summary>
    public interface IAliPayRepository : IBaseRepository<Setting>
    {
    }
}