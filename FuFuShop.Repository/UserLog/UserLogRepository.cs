
using CoreCms.Net.IRepository;
using FuFuShop.Model.Entitys;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace CoreCms.Net.Repository
{
    /// <summary>
    ///     用户日志 接口实现
    /// </summary>
    public class UserLogRepository : BaseRepository<UserLog>, IUserLogRepository
    {
        public UserLogRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}