
using FuFuShop.Model.Entities;
using FuFuShop.Repository.Bill;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services.Bill
{
    /// <summary>
    /// 发货单表 接口实现
    /// </summary>
    public class BillDeliveryServices : BaseServices<BillDelivery>, IBillDeliveryServices
    {
        private readonly IBillDeliveryRepository _dal;
        private readonly IBillDeliveryItemServices _billDeliveryItemServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;



        public BillDeliveryServices(
            IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider
            , IBillDeliveryRepository dal
            , IBillDeliveryItemServices billDeliveryItemServices)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _billDeliveryItemServices = billDeliveryItemServices;
        }


    }
}