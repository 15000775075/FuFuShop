using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace CoreCms.Net.IServices
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
    }
}