

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     操作日志表 接口实现
    /// </summary>
    public class SysOperRecordRepository : BaseRepository<SysOperRecord>, ISysOperRecordRepository
    {
        public SysOperRecordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}