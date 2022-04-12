
using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;
using FuFuShop.Repository.UnitOfWork;

namespace FuFuShop.Repository
{
    /// <summary>
    /// 提交表单保存大文本值表 接口实现
    /// </summary>
    public class FormSubmitDetailRepository : BaseRepository<FormSubmitDetail>, IFormSubmitDetailRepository
    {
        public FormSubmitDetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }
}
