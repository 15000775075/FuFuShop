
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services;
using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     广告表 接口实现
    /// </summary>
    public class AdvertisementServices : BaseServices<Advertisement>, IAdvertisementServices
    {
        private readonly IAdvertisementRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public AdvertisementServices(IUnitOfWork unitOfWork, IAdvertisementRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}