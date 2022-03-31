
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品收藏
    /// </summary>
    public partial class GoodsCollection
    {
        /// <summary>
        ///     商品信息
        /// </summary>
        [Display(Name = "商品信息")]
        [SugarColumn(IsIgnore = true)]
        public Goods goods { get; set; }
    }
}