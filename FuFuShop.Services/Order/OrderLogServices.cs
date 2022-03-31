using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     订单记录表 接口实现
    /// </summary>
    public class OrderLogServices : BaseServices<OrderLog>, ILogServices
    {
        private readonly IOrderLogRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public OrderLogServices(IUnitOfWork unitOfWork, IOrderLogRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}