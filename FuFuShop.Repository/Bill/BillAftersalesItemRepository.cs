
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     售后单明细表 接口实现
    /// </summary>
    public class BillAftersalesItemRepository : BaseRepository<BillAftersalesItem>,
        IBillAftersalesItemRepository
    {
        public BillAftersalesItemRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}