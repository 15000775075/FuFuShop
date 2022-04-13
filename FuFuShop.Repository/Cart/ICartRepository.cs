using FuFuShop.Model.Entities.Cart;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     购物车表 工厂接口
    /// </summary>
    public interface ICartRepository : IBaseRepository<Cart>
    {
        /// <summary>
        ///     获取购物车用户数据总数
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync(int userId);
    }
}