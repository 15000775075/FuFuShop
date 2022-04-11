using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     支付方式表 接口实现
    /// </summary>
    public class PaymentsRepository : BaseRepository<Payments>, IPaymentsRepository
    {
        public PaymentsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}