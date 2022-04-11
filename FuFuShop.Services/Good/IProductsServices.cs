using FuFuShop.Model.Entities;
using FuFuShop.Services.BaseServices;

namespace FuFuShop.Services.Good
{
    /// <summary>
    ///     货品表 服务工厂接口
    /// </summary>
    public interface IProductsServices : IBaseServices<Products>
    {

        /// <summary>
        ///     根据货品ID获取货品信息
        /// </summary>
        /// <param name="id">货品序列</param>
        /// <param name="isPromotion">是否计算促销</param>
        /// <param name="userId">用户序列</param>
        /// <param name="type">类型</param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<Products> GetProductInfo(int id, bool isPromotion, int userId, string type = "goods",
            int groupId = 0);

        /// <summary>
        ///     判断货品上下架状态
        /// </summary>
        /// <param name="productsId">货品序列</param>
        /// <returns></returns>
        Task<bool> GetShelfStatus(int productsId);

    }
}