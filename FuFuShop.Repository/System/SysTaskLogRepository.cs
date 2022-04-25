

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     定时任务日志 接口实现
    /// </summary>
    public class SysTaskLogRepository : BaseRepository<SysTaskLog>, ISysTaskLogRepository
    {
        public SysTaskLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}