using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     品牌表 接口实现
    /// </summary>
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}