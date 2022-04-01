using CoreCms.Net.IRepository;
using CoreCms.Net.IServices;
using FuFuShop.Model.Entities;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 商品分类扩展表 接口实现
    /// </summary>
    public class GoodsCategoryExtendServices : BaseServices<GoodsCategoryExtend>, IGoodsCategoryExtendServices
    {
        private readonly IGoodsCategoryExtendRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public GoodsCategoryExtendServices(IUnitOfWork unitOfWork, IGoodsCategoryExtendRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
