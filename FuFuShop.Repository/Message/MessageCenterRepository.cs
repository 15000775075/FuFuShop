
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 消息配置表 接口实现
    /// </summary>
    public class MessageCenterRepository : BaseRepository<MessageCenter>, IMessageCenterRepository
    {
        public MessageCenterRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
