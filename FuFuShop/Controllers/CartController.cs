using FuFuShop.Common.Auth.HttpContextUser;
using FuFuShop.Services;
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


    }
}