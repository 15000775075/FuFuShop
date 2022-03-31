using FuFuShop.Model.Entitys;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     购物车表 工厂接口
    /// </summary>
    public interface ICartRepository : IBaseRepository<Cart>
    {
    }
}