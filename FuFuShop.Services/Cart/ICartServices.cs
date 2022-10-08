using FuFuShop.Common.AppSettings;
using FuFuShop.Model.Entities.Cart;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services
{
    /// <summary>
    ///     购物车表 服务工厂接口
    /// </summary>
    public interface ICartServices : IBaseServices<Cart>
    {
        /// <summary>
        ///     设置购物车商品数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nums"></param>
        /// <param name="userId"></param>
        /// <param name="numType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<WebApiCallBack> SetCartNum(int id, int nums, int userId, int numType, int type = 1);


        /// <summary>
        ///     重写删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<WebApiCallBack> DeleteByIdsAsync(int id, int userId);


        /// <summary>
        ///     添加单个货品到购物车
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="productId">货品序号</param>
        /// <param name="nums">数量</param>
        /// <param name="numType">数量类型/1是直接增加/2是赋值</param>
        /// <param name="cartTypes">1普通购物/2拼团模式/3团购模式/4秒杀模式/6砍价模式/7赠品</param>
        /// <param name="objectId">关联对象类型</param>
        /// <returns></returns>
        Task<WebApiCallBack> Add(int userId, int productId, int nums, int numType, int cartTypes = 1, int objectId = 0);



        /// <summary>
        ///     获取购物车列表
        /// </summary>
        /// <param name="userId">用户序号</param>
        /// <param name="ids">已选择货号</param>
        /// <param name="type">购物车类型/同订单类型</param>
        /// <param name="objectId">关联非订单类型数据序列</param>
        /// <returns></returns>
        Task<WebApiCallBack> 
            {
            (int userId, int[] ids = null, int type = 1, int objectId = 0);



        /// <summary>
        ///     获取处理后的购物车信息
        /// </summary>
        /// <param name="userId">用户序列</param>
        /// <param name="ids">选中的购物车商品</param>
        /// <param name="orderType">订单类型</param>
        /// <param name="areaId">收货地址id</param>
        /// <param name="point">消费的积分</param>
        /// <param name="couponCode">优惠券码</param>
        /// <param name="freeFreight">是否免运费</param>
        /// <param name="deliveryType">关联上面的是否免运费/1=快递配送（要去算运费）生成订单记录快递方式，2=同城配送/3=门店自提（不需要计算运费）生成订单记录门店自提信息</param>
        /// <param name="objectId">关联非普通订单营销类型序列</param>
        /// <returns></returns>
        Task<WebApiCallBack> GetCartInfos(int userId, int[] ids, int orderType, int areaId, int point,
            string couponCode, bool freeFreight = false,
            int deliveryType = (int)GlobalEnumVars.OrderReceiptType.Logistics, int objectId = 0);


        /// <summary>
        ///     获取购物车用户数据总数
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync(int userId);


        /// <summary>
        /// Gets the cart dto data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ids">The ids.</param>
        /// <param name="type">The type.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns></returns>
        Task<WebApiCallBack> GetCartDtoData(int userId, int[] ids = null, int type = 1, int objectId = 0);
    }
}