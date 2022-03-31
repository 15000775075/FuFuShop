

using CoreCms.Net.IRepository;
using CoreCms.Net.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace CoreCms.Net.Repository
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