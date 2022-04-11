using FuFuShop.Model.Entities.Bill;
using FuFuShop.Repository.Bill;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Bill
{
    /// <summary>
    ///     发货单详情表 接口实现
    /// </summary>
    public class BillDeliveryItemServices : BaseServices<BillDeliveryItem>,
        IBillDeliveryItemServices
    {
        private readonly IBillDeliveryItemRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public BillDeliveryItemServices(IUnitOfWork unitOfWork, IBillDeliveryItemRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}