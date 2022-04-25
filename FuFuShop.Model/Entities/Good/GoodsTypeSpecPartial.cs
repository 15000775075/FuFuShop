

using SqlSugar;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品类型属性表
    /// </summary>
    public partial class GoodsTypeSpec
    {
        /// <summary>
        ///     子类
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<GoodsTypeSpecValue> specValues { get; set; } = new();
    }
}