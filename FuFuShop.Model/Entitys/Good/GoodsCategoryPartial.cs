
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    ///     商品分类
    /// </summary>
    public partial class GoodsCategory
    {
        /// <summary>
        ///     类别名称
        /// </summary>
        [Display(Name = "类别名称")]
        [SugarColumn(IsIgnore = true)]
        public string typeName { get; set; }
    }
}