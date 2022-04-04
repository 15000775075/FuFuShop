using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities.Good
{
    /// <summary>
    /// 商品分类扩展表
    /// </summary>
    [SugarTable("FuFuShop_GoodsCategoryExtend", TableDescription = "商品分类扩展表")]
    public partial class GoodsCategoryExtend
    {
        /// <summary>
        /// 商品分类扩展表
        /// </summary>
        public GoodsCategoryExtend()
        {
        }

        /// <summary>
        /// 序列
        /// </summary>
        [Display(Name = "序列")]
        [SugarColumn(ColumnDescription = "序列", IsPrimaryKey = true, IsIdentity = true)]
        [Required(ErrorMessage = "请输入{0}")]
        public int id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [Display(Name = "商品id")]
        [SugarColumn(ColumnDescription = "商品id", IsNullable = true)]
        public int? goodsId { get; set; }
        /// <summary>
        /// 商品分类id
        /// </summary>
        [Display(Name = "商品分类id")]
        [SugarColumn(ColumnDescription = "商品分类id", IsNullable = true)]
        public int? goodsCategroyId { get; set; }
    }
}