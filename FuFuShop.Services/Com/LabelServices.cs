

using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     标签表 接口实现
    /// </summary>
    public class LabelServices : BaseServices<Label>, ILabelServices
    {
        private readonly ILabelRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public LabelServices(IUnitOfWork unitOfWork, ILabelRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}