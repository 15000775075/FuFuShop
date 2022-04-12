
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     商品图片关联表 接口实现
    /// </summary>
    public class BillAftersalesImagesServices : BaseServices<BillAftersalesImages>,
        IBillAftersalesImagesServices
    {
        private readonly IBillAftersalesImagesRepository _dal;
        private readonly IUnitOfWork _unitOfWork;

        public BillAftersalesImagesServices(IUnitOfWork unitOfWork, IBillAftersalesImagesRepository dal)
        {
            _dal = dal;
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }
    }
}