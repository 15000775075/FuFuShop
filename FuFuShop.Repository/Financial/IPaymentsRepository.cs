

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     支付方式表 工厂接口
    /// </summary>
    public interface IPaymentsRepository : IBaseRepository<Payments>
    {
    }
}