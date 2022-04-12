

using CoreCms.Net.IRepository;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 消息发送表 接口实现
    /// </summary>
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
