using FuFuShop.Model.Entities.Bill;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository.Bill
{
    /// <summary>
    ///     发货单详情表 接口实现
    /// </summary>
    public class BillDeliveryItemRepository : BaseRepository<BillDeliveryItem>,
        IBillDeliveryItemRepository
    {
        public BillDeliveryItemRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}