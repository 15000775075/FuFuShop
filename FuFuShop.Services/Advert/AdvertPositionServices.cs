
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services;
using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     广告位置表 接口实现
    /// </summary>
    public class AdvertPositionServices : BaseServices<AdvertPosition>, IAdvertPositionServices
    {
        private readonly IAdvertPositionRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public AdvertPositionServices(IUnitOfWork unitOfWork, IAdvertPositionRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}