using FuFuShop.Model.Entities.Good;
using FuFuShop.Repository.Good;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Good
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
            BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
