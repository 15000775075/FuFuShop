using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository.Shop
{
    /// <summary>
    /// 物流公司表 接口实现
    /// </summary>
    public class LogisticsRepository : BaseRepository<Logistics>, ILogisticsRepository
    {
        public LogisticsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }
}
