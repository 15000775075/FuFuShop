using FuFuShop.Model.Entities;
using FuFuShop.Model.ViewModels.Basics;
using FuFuShop.Model.ViewModels.UI;
using FuFuShop.Services.BaseServices;
using SqlSugar;
using System.Linq.Expressions;

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

        /// <summary>
        ///     获取库存报警数量
        /// </summary>
        /// <param name="goodsStocksWarn"></param>
        /// <returns></returns>
        Task<int> GoodsStaticsTotalWarn(int goodsStocksWarn);

        /// <summary>
        ///     获取所有货品数据
        /// </summary>
        /// <returns></returns>
        Task<List<Products>> GetProducts(int goodId = 0);

        /// <summary>
        ///     修改单个货品库存并记入库存管理日志内
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        Task<AdminUiCallBack> EditStock(int productId, int stock);



        /// <summary>
        ///     获取关联商品的货品列表数据
        /// </summary>
        /// <param name="predicate">判断集合</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <param name="orderByExpression"></param>
        /// <param name="blUseNoLock">是否使用WITH(NOLOCK)</param>
        /// <returns></returns>
        Task<IPageList<Products>> QueryDetailPageAsync(Expression<Func<Products, bool>> predicate,
            Expression<Func<Products, object>> orderByExpression, OrderByType orderByType, int pageIndex = 1,
            int pageSize = 20, bool blUseNoLock = false);

    }
}