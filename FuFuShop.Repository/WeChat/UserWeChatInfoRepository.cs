using FuFuShop.Model.Entitys;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository.WeChat
{
    /// <summary>
    ///     用户表 接口实现
    /// </summary>
    public class UserWeChatInfoRepository : BaseRepository<UserWeChatInfo>,
        IUserWeChatInfoRepository
    {
        public UserWeChatInfoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}