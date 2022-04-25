
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;
namespace FuFuShop.Repository
{
    /// <summary>
    /// 数据字典项表 接口实现
    /// </summary>
    public class SysDictionaryDataRepository : BaseRepository<SysDictionaryData>, ISysDictionaryDataRepository
    {
        public SysDictionaryDataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


    }
}
