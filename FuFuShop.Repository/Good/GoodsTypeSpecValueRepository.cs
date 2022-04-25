

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品类型属性值表 接口实现
    /// </summary>
    public class GoodsTypeSpecValueRepository : BaseRepository<GoodsTypeSpecValue>, IGoodsTypeSpecValueRepository
    {
        public GoodsTypeSpecValueRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
