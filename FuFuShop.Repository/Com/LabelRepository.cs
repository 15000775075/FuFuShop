

using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    ///     标签表 接口实现
    /// </summary>
    public class LabelRepository : BaseRepository<Label>, ILabelRepository
    {
        public LabelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}