using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.Repository.Good
{
    /// <summary>
    ///     商品表 工厂接口
    /// </summary>
    public interface IGoodsRepository : IBaseRepository<Goods>
    {
        /// <summary>
        ///     获取随机推荐数据
        /// </summary>
        /// <param name="number">数量</param>
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