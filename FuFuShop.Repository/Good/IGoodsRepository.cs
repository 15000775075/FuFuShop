using FuFuShop.Model.Entities;
using FuFuShop.Repository.BaseRepository;

namespace FuFuShop.IRepository
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
    }
}