using FuFuShop.Model.Entitys;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     购物车表 接口实现
    /// </summary>
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }
}