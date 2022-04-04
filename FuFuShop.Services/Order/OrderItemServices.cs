using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 订单明细表 接口实现
    /// </summary>
    public class OrderItemServices : BaseServices<OrderItem>, IOrderItemServices
    {
        private readonly IOrderItemRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public OrderItemServices(IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider,
            IOrderItemRepository dal)
        {
            _serviceProvider = serviceProvider;
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }



    }
}
