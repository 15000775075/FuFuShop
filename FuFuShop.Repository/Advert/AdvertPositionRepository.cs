
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

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