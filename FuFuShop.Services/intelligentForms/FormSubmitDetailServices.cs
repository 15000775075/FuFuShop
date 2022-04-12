
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 提交表单保存大文本值表 接口实现
    /// </summary>
    public class FormSubmitDetailServices : BaseServices<FormSubmitDetail>, IFormSubmitDetailServices
    {
        private readonly IFormSubmitDetailRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public FormSubmitDetailServices(IUnitOfWork unitOfWork, IFormSubmitDetailRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
