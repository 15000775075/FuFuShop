using FuFuShop.Model.Entities.Shop;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository.Shop
{
    /// <summary>
    /// 店铺设置表 接口实现
    /// </summary>
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
