using CoreCms.Net.IRepository;
using CoreCms.Net.IServices;
using CoreCms.Net.Model.Entities;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;


namespace FuFuShop.Services
{
    /// <summary>
    /// 发货单表 接口实现
    /// </summary>
    public class CoreCmsBillDeliveryServices : BaseServices<CoreCmsBillDelivery>, ICoreCmsBillDeliveryServices
    {
        private readonly IBillDeliveryRepository _dal;
        private readonly IBillDeliveryItemServices _billDeliveryItemServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;



        public CoreCmsBillDeliveryServices(
            IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider
            , IBillDeliveryRepository dal
            , IBillDeliveryItemServices billDeliveryItemServices)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _billDeliveryItemServices = billDeliveryItemServices;
        }


    }
}