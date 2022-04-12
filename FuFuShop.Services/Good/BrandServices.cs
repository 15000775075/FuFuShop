using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 品牌表 接口实现
    /// </summary>
    public class BrandServices : BaseServices<Brand>, IBrandServices
    {
        private readonly IBrandRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public BrandServices(IUnitOfWork unitOfWork, IBrandRepository dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }

    }
}
