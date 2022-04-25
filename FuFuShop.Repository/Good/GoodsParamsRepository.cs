
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品参数表 接口实现
    /// </summary>
    public class GoodsParamsRepository : BaseRepository<GoodsParams>, IGoodsParamsRepository
    {
        public GoodsParamsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
