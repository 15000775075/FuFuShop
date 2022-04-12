
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Model.Entities;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     广告表 接口实现
    /// </summary>
    public class AdvertisementRepository : BaseRepository<Advertisement>, IAdvertisementRepository
    {
        public AdvertisementRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}