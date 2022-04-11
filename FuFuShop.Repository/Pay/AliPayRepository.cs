
using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 支付宝支付 接口实现
    /// </summary>
    public class AliPayRepository : BaseRepository<Setting>, IAliPayRepository
    {
        public AliPayRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}