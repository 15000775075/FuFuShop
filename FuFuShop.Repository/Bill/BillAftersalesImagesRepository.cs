

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     商品图片关联表 接口实现
    /// </summary>
    public class BillAftersalesImagesRepository : BaseRepository<BillAftersalesImages>,
        IBillAftersalesImagesRepository
    {
        public BillAftersalesImagesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}