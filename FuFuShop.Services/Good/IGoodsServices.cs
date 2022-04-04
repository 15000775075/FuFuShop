using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Good
{
    /// <summary>
    ///     商品表 服务工厂接口
    /// </summary>
    public interface IGoodsServices : IBaseServices<Goods>
    {
        /// <summary>
        ///     获取商品详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="isPromotion"></param>
        /// <param name="type"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<Goods> GetGoodsDetial(int id, int userId = 0, bool isPromotion = false, string type = "goods",
            int groupId = 0);


        /// <summary>
        ///     获取随机推荐数据
        /// </summary>
        /// <param name="number"></param>
        /// <param name="isRecommend">是否推荐</param>
        /// <returns></returns>
        Task<List<Goods>> GetGoodsRecommendList(int number, bool isRecommend = false);



        /// <summary>
        ///     库存改变机制。
        ///     库存机制：商品下单 总库存不变，冻结库存加1，
        ///     商品发货：冻结库存减1，总库存减1，
        ///     订单完成但未发货：总库存不变，冻结库存减1
        ///     商品退款&取消订单：总库存不变，冻结库存减1,
        ///     商品退货：总库存加1，冻结库存不变,
        ///     可销售库存：总库存-冻结库存
        /// </summary>
        /// <returns></returns>
        WebApiCallBack ChangeStock(int productsId, string type = "order", int num = 0);
    }
}