

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     Nlog记录表 接口实现
    /// </summary>
    public class SysNLogRecordsRepository : BaseRepository<SysNLogRecords>, ISysNLogRecordsRepository
    {
        public SysNLogRecordsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}