using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     退货单明细表 接口实现
    /// </summary>
    public class BillReshipItemServices : BaseServices<BillReshipItem>, IBillReshipItemServices
    {
        private readonly IBillReshipItemRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public BillReshipItemServices(IUnitOfWork unitOfWork, IBillReshipItemRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}