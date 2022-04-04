

using SqlSugar;

namespace FuFuShop.Model.Entities.Shop
{
    /// <summary>
    ///     配送方式表
    /// </summary>
    public partial class Ship
    {
        /// <summary>
        ///     商品总额满多少免运费
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public object areaFeeObj { get; set; } = null;
    }
}