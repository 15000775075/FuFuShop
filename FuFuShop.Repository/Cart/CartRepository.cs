using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities;
using FuFuShop.Model.Entities.Cart;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
using SqlSugar;

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
        #region 获取购物车用户数据总数

        /// <summary>
        ///     获取购物车用户数据总数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountAsync(int userId)
        {
            var count = DbClient.Queryable<Cart, Products, Goods>((cart, products, goods) =>
                    new object[]
                    {
                        JoinType.Inner, cart.productId == products.id,
                        JoinType.Inner, products.goodsId == goods.id
                    })
                .Where((cart, products, goods) => cart.type == (int)GlobalEnumVars.OrderType.Common)
                .Select((cart, products, goods) => new { cart.id, cart.userId, goodId = goods.id })
                .MergeTable()
                .CountAsync(p => p.userId == userId);
            return await count;
        }

        #endregion
    }
}