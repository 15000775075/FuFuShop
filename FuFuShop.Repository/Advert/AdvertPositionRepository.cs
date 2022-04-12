using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     广告位置表 接口实现
    /// </summary>
    public class AdvertPositionRepository : BaseRepository<AdvertPosition>,
        IAdvertPositionRepository
    {
        public AdvertPositionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}