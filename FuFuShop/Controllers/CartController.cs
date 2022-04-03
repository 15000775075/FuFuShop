using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Common.Helper;
using FuFuShop.Model.FromBody;
using FuFuShop.Model.ViewModels.DTO;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuFuShop.Controllers
{
    /// <summary>
    /// 购物车操作
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IHttpContextUser _user;
        private readonly ICartServices _cartServices;


        /// <summary>
        /// 构造函数
        /// </summary>
        public CartController(IHttpContextUser user, ICartServices cartServices)
        {
            _user = user;
            _cartServices = cartServices;
        }

        /// <summary>
        /// 添加单个货品到购物车
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> AddCart([FromBody] FMCartAdd entity)
        {
            var jm = await _cartServices.Add(_user.ID, entity.ProductId, entity.Nums, entity.type, entity.cartType, entity.objectId);
            return jm;
        }


        /// <summary>
        /// 获取购物车列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> GetList([FromBody] FMCartGetList entity)
        {
            var ids = CommonHelper.StringToIntArray(entity.ids);
            //判断免费运费
            var freeFreight = entity.receiptType != 1;
            //获取数据
            var jm = await _cartServices.GetCartInfos(_user.ID, ids, entity.type, entity.areaId, entity.point, entity.couponCode, freeFreight, entity.receiptType, entity.objectId);

            return jm;
        }

        /// <summary>
        /// 获取购物车列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> DoDelete([FromBody] FMIntId entity)
        {
            var jm = new WebApiCallBack();

            if (entity.id <= 0)
            {
                jm.msg = "请提交要删除的货品";
                return jm;
            }
            jm = await _cartServices.DeleteByIdsAsync(entity.id, _user.ID);

            return jm;
        }


        /// <summary>
        /// 设置购物车商品数量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<WebApiCallBack> SetCartNum([FromBody] FMSetCartNum entity)
        {
            var jm = await _cartServices.SetCartNum(entity.id, entity.nums, _user.ID, 2, 1);
            return jm;
        }



    }
}