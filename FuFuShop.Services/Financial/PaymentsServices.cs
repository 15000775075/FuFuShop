using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 支付方式表 接口实现
    /// </summary>
    public class PaymentsServices : BaseServices<Payments>, IPaymentsServices
    {
        private readonly IPaymentsRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentsServices(IUnitOfWork unitOfWork, IPaymentsRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
