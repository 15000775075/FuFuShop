
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     登录日志表 接口实现
    /// </summary>
    public class SysLoginRecordRepository : BaseRepository<SysLoginRecord>, ISysLoginRecordRepository
    {
        public SysLoginRecordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}