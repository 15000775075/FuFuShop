
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace FuFuShop.Model.Entities
{
    /// <summary>
    /// 商品图片关联表
    /// </summary>
    [SugarTable("FuFuShop_GoodsImages", TableDescription = "商品图片关联表")]
    public partial class GoodsImages
    {
        /// <summary>
        /// 商品图片关联表
        /// </summary>
        public GoodsImages()
        {
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        [Display(Name = "商品ID")]
        [SugarColumn(ColumnDescription = "商品ID")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Int32 goodsId { get; set; }
        /// <summary>
        /// 图片ID
        /// </summary>
        [Display(Name = "图片ID")]
        [SugarColumn(ColumnDescription = "图片ID")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, ErrorMessage = "【{0}】不能超过{1}字符长度")]
        public System.String imageId { get; set; }
        /// <summary>
        /// 图片排序
        /// </summary>
        [Display(Name = "图片排序")]
        [SugarColumn(ColumnDescription = "图片排序")]
        [Required(ErrorMessage = "请输入{0}")]
        public System.Int32 sort { get; set; }
    }
}