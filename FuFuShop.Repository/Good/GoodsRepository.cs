
using FuFuShop.IRepository;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 商品表 接口实现
    /// </summary>
    public class GoodsRepository : BaseRepository<Goods>, IGoodsRepository
    {
        public GoodsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }




    }
}
