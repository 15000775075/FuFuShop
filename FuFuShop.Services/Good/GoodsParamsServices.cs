
using FuFuShop.Model.Entities;
using FuFuShop.Repository;
using FuFuShop.Repository.UnitOfWork;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    /// 商品参数表 接口实现
    /// </summary>
    public class GoodsParamsServices : BaseServices<GoodsParams>, IGoodsParamsServices
    {
        private readonly IGoodsParamsRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        public GoodsParamsServices(IUnitOfWork unitOfWork, IGoodsParamsRepository dal)
        {
            _dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
        }


    }
}
