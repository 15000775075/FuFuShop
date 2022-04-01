
using CoreCms.Net.IRepository;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     商品分类扩展表 接口实现
    /// </summary>
    public class GoodsCategoryExtendRepository : BaseRepository<GoodsCategoryExtend>,
        IGoodsCategoryExtendRepository
    {
        public GoodsCategoryExtendRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}