

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    ///     数据字典表 接口实现
    /// </summary>
    public class SysDictionaryRepository : BaseRepository<SysDictionary>, ISysDictionaryRepository
    {
        public SysDictionaryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}